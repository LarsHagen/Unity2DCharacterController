using UnityEngine;

namespace Assets.Scripts.PlayerSystem
{
    public interface ICharacterController2D
    {
        /// <summary>
        /// Is the character controller currently grounded
        /// </summary>
        bool Grounded { get; }

        /// <summary>
        /// The current velocity of the character controller
        /// </summary>
        Vector2 Velocity { get; }

        /// <summary>
        /// The current velocity relative to any elevator that the character controller might be standing on. If the character controller is not on an elevator then the return value should be equal to ICharacterController2D.Velocity
        /// </summary>
        Vector2 VelocityRelativeToElevator { get; }

        /// <summary>
        /// Instruct the character controller to jump
        /// </summary>
        void Jump();

        /// <summary>
        /// Send move input to the character controller
        /// </summary>
        void Move(Vector2 move);
    }
}