using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { get { return _instance; } }
    private static CameraController _instance;

    public SpaceCollisionsController Target;
    public float VerticalOffset;
    public float LookAheadDstX;
    public float LookSmoothTimeX;
    public float VerticalSmoothTime;
    public Vector2 FocusAreaSize;
    public float Zoom;

	FocusArea focusArea;

    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirX;
    private float smoothLookVelocityX;
    private float smoothVelocityY;

    private bool lookAheadStopped;
    private Camera _camera;
    private bool _is2dView;
    private Vector3 _currentRotation;
    private Vector3 _2DRotation = Vector3.zero;
    private Vector3 _3DRotation = new Vector3(10,0,0);

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _camera = GetComponent<Camera>();
        _is2dView = true;
    }

    public void SetTarget(GameObject targetGo)
    {
        Target = targetGo.GetComponentInChildren<SpaceCollisionsController>() ;
        focusArea = new FocusArea(Target.boxCollider.bounds, FocusAreaSize);
        _camera.orthographicSize = Zoom;
    }
    public void SwitchView( )
    {
        Debug.Log("SwitchView " + _is2dView);
        _is2dView  = !_is2dView;
        _currentRotation = _is2dView ? _2DRotation : _3DRotation;
        VerticalOffset = _is2dView ? 1 : 2;
    }

    void Start() {

        if (Target == null) return;
	}

	void LateUpdate() {
        if (Target == null) return;
		focusArea.Update (Target.boxCollider.bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * VerticalOffset;

		if (focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
			if (Mathf.Sign(Target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && Target.playerInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * LookAheadDstX;
			}
			else {
				if (!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * LookAheadDstX - currentLookAheadX)/4f;
				}
			}
		}


		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, LookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, VerticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_currentRotation), 10* Time.deltaTime);
    }

	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 0, .5f);
		Gizmos.DrawCube (focusArea.centre, FocusAreaSize);
	}

	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;
		float left,right;
		float top,bottom;

		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left+right)/2,(top +bottom)/2);
		}

		public void Update(Bounds targetBounds) {

			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left+right)/2,(top +bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}
}
