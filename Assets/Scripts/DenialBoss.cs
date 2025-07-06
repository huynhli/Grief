using UnityEngine;

public class DenialBoss : Enemy
{
    [Header("Boss Phases")]
    public Player player;
    private Animator animator;
    private bool isInvulnerable = true;
    private bool battleStarted = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip introMusic;
    public AudioClip battleMusic;

    [Header("Intro Animation")]
    public float introAnimationDuration = 24f;
}
