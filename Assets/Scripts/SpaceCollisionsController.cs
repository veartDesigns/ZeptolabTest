using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class SpaceCollisionsController : MonoBehaviour
{
    public LayerMask collisionMask;
    public CollisionInfo collisionsInfo;
    public Vector2 playerInput;
    public BoxCollider boxCollider;
    private CharacterBoundPoints _characterBoundVerts;

    private const float _boundsOffsetWidth = .015f;
    private float _distBetweenRays = .25f;
    private int _horizontalRaysNum;
    private int _verticalRaysNum;
    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        CalculateRaySpacing();
        collisionsInfo.faceDir = 1;
    }

    public void GetCharacterVerts()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(_boundsOffsetWidth * -2);

        _characterBoundVerts.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _characterBoundVerts.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _characterBoundVerts.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _characterBoundVerts.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(_boundsOffsetWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;
        _horizontalRaysNum = Mathf.CeilToInt(boundsHeight / _distBetweenRays);
        _verticalRaysNum = Mathf.CeilToInt(boundsWidth / _distBetweenRays);
        _horizontalRaySpacing = bounds.size.y / (_horizontalRaysNum - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRaysNum - 1);
    }

    public struct CharacterBoundPoints
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    float maxClimbAngle = 80;
    float maxDescendAngle = 80;

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        GetCharacterVerts();

        collisionsInfo.Reset();
        collisionsInfo.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.x != 0)
        {
            collisionsInfo.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisionsInfo.below = true;
        }
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisionsInfo.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + _boundsOffsetWidth;

        if (Mathf.Abs(moveAmount.x) < _boundsOffsetWidth)
        {
            rayLength = 2 * _boundsOffsetWidth;
        }

        for (int i = 0; i < _horizontalRaysNum; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? _characterBoundVerts.bottomLeft : _characterBoundVerts.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
            RaycastHit hit;
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (Physics.Raycast(rayOrigin, Vector2.right * directionX, out hit, rayLength, collisionMask))
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisionsInfo.descendingSlope)
                    {
                        collisionsInfo.descendingSlope = false;
                        moveAmount = collisionsInfo.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionsInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _boundsOffsetWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisionsInfo.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    moveAmount.x = (hit.distance - _boundsOffsetWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisionsInfo.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisionsInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisionsInfo.left = directionX == -1;
                    collisionsInfo.right = directionX == 1;
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + _boundsOffsetWidth;

        for (int i = 0; i < _verticalRaysNum; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _characterBoundVerts.bottomLeft : _characterBoundVerts.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + moveAmount.x);

            RaycastHit hit;
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (Physics.Raycast(rayOrigin, Vector2.up * directionY, out hit, rayLength, collisionMask))
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisionsInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisionsInfo.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }

                moveAmount.y = (hit.distance - _boundsOffsetWidth) * directionY;
                rayLength = hit.distance;

                if (collisionsInfo.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisionsInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisionsInfo.below = directionY == -1;
                collisionsInfo.above = directionY == 1;
            }
        }

        if (collisionsInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + _boundsOffsetWidth;
            Vector2 rayOrigin = ((directionX == -1) ? _characterBoundVerts.bottomLeft : _characterBoundVerts.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionsInfo.slopeAngle)
                {
                    moveAmount.x = (hit.distance - _boundsOffsetWidth) * directionX;
                    collisionsInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisionsInfo.below = true;
            collisionsInfo.climbingSlope = true;
            collisionsInfo.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1) ? _characterBoundVerts.bottomRight : _characterBoundVerts.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - _boundsOffsetWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisionsInfo.slopeAngle = slopeAngle;
                        collisionsInfo.descendingSlope = true;
                        collisionsInfo.below = true;
                    }
                }
            }
        }
    }

    private void ResetFallingThroughPlatform()
    {
        collisionsInfo.fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
