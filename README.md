# Unity2DCharacterController
A general purpose 2D CharacterController using Rigidbody2D

## How to use
Attach CharacterController2D to a gameobject with a Rigidbody2D component and a CapsuleCollider2D component. Change settings for the character controller in the inspector.

### Elevators
The character contoller can work with elevators. To make this work you need to implement a new monobehaviour class that implements IElevator. You decide how this class should be implemented and move around. The character controller just needs to be able to read the velocity of the elevator through the interface and also be able to hit the elevator with a physics2d raycast.