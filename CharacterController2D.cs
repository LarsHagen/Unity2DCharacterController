using UnityEngine;

namespace Assets.Scripts.PlayerSystem
{
    public class CharacterController2D : MonoBehaviour, ICharacterController2D
    {
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _speed;
        [SerializeField] private float _accelerationGround;
        [SerializeField] private float _accelerationAir;
        [SerializeField] private float _maxSlopeAngle;
        [SerializeField] private LayerMask _walkableLayerMask;

        private Rigidbody2D _rigidbody2D;
        private Collider2D _capsuleCollider2D;
        private IElevator _currentElevator;

        public bool Grounded { get; private set; }
        public Vector2 Velocity => _rigidbody2D.velocity;
        public Vector2 VelocityRelativeToElevator { get; private set; }

        private bool _jumpedPressed;
        private Vector2 _moveInput;
        private Vector2 _groundNormal;
        private float _moveSpeed;
        private float _gravityScale;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _capsuleCollider2D = GetComponent<Collider2D>();
            _gravityScale = _rigidbody2D.gravityScale;
        }

        private void FixedUpdate()
        {
            CalculateIsGrounded();
            _rigidbody2D.gravityScale = Grounded ? 0 : _gravityScale;

            float velocityX = _rigidbody2D.velocity.x;
            float velocityY = _rigidbody2D.velocity.y;

            HandleMoveInput(ref velocityX, ref velocityY);
            HandleElevatorMovement(ref velocityX, ref velocityY);
            HandleJumping(ref velocityY);

            _rigidbody2D.velocity = new Vector2(velocityX, velocityY);

            if (_currentElevator != null)
            {
                VelocityRelativeToElevator = _rigidbody2D.velocity - new Vector2(_currentElevator.ElevatorVelocityX, _currentElevator.ElevatorVelocityY);
            }
            else
            {
                VelocityRelativeToElevator = _rigidbody2D.velocity;
            }
        }

        private void HandleJumping(ref float velocityY)
        {
            if (_jumpedPressed)
            {
                _jumpedPressed = false;

                if (Grounded)
                    velocityY = _jumpForce;
            }
        }

        private void HandleElevatorMovement(ref float velocityX, ref float velocityY)
        {
            if (_currentElevator != null)
            {
                velocityX += _currentElevator.ElevatorVelocityX;
                velocityY += _currentElevator.ElevatorVelocityY;
            }
        }

        private void HandleMoveInput(ref float velocityX, ref float velocityY)
        {
            if (Grounded)
            {
                var groundTangent = (Quaternion.AngleAxis(90, Vector3.back) * _groundNormal).normalized;
                var currentSpeedOnGroundNormal = Vector3.Project(_rigidbody2D.velocity, groundTangent).magnitude;

                //Clamp move velocity to be maximum of the current speed we are moving. We could have hit a wall for example
                _moveSpeed = Mathf.Clamp(_moveSpeed, -currentSpeedOnGroundNormal, currentSpeedOnGroundNormal);
                _moveSpeed = Mathf.MoveTowards(_moveSpeed, _moveInput.x * _speed, Time.fixedDeltaTime * _accelerationGround);
                var moveRelativeToGroundNormal = groundTangent * _moveSpeed;
                velocityX = moveRelativeToGroundNormal.x;
                velocityY = moveRelativeToGroundNormal.y;
            }
            else
            {
                //Clamp move velocity to be maximum of the current x velocity. We could have hit a wall for example
                _moveSpeed = Mathf.Clamp(_moveSpeed, -Mathf.Abs(velocityX), Mathf.Abs(velocityX));

                _moveSpeed = Mathf.MoveTowards(_moveSpeed, _moveInput.x * _speed, Time.fixedDeltaTime * _accelerationAir);
                velocityX = _moveSpeed;
            }
        }

        private void CalculateIsGrounded()
        {
            var radius = _capsuleCollider2D.bounds.size.x / 2f - 0.1f;
            var start = _capsuleCollider2D.bounds.center;
            var distance = _capsuleCollider2D.bounds.extents.y - radius + 0.2f;
            var hit = Physics2D.CircleCast(start, radius, Vector2.down, distance, _walkableLayerMask);

            _groundNormal = hit.normal;
            
            if (hit.collider != null)
            {
                var angle = Vector2.Angle(_groundNormal, Vector2.up);
                Grounded = angle <= _maxSlopeAngle;
            }
            else
            {
                Grounded = false;
            }

            _currentElevator = hit.transform?.GetComponent<IElevator>();
        }

        public void Jump()
        {
            _jumpedPressed = true;
        }

        public void Move(Vector2 move)
        {
            _moveInput = move;
        }
    }
}