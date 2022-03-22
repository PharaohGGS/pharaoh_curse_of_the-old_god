using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementData", menuName = "Data/PlayerMovement")]
public class PlayerMovementData : ScriptableObject
{

    [Header("Horizontal Movement")]

    [Tooltip("Grounded horizontal speed (m/s)")]
    public float horizontalSpeed = 5f;
    [Tooltip("In-air horizontal speed (m/s)")]
    public float inAirHorizontalSpeed = 5f;
    [Tooltip("NOCLIP mode speed (m/s)")]
    public float noclipSpeed = 10f;
    [Tooltip("How long the player is stunned when getting damaged")]
    public float respawnStunDuration = 1.5f;

    [Header("Jump")]

    [Tooltip("Defines the force added to the player when initiating the jump")]
    public float initialJumpForce = 11f;
    [Tooltip("Defines the force added to the player while holding the jump button")]
    public float heldJumpForce = 18.5f;
    [Tooltip("How long the player can hold down the jump button after jumping")]
    public float maxJumpHold = 0.4f;

    [Header("Dash")]

    [Tooltip("Dashing force")]
    public float dashForce = 35f;
    [Tooltip("Dash duration")]
    public float dashDuration = 0.1f;
    [Tooltip("Cooldown between each dash, starts at the end of the previous one")]
    public float dashCooldown = 0.5f;

}
