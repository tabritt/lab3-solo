using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player;

    [Header("Orbit Behavior")]
    public float speedMultip = 10f;     //Distancebased orbiting 
    public float EnemyLag = 2f;          //so enemys don't isntantly follow up to players movement

    [Header("Collision Avoidance")]
    public float EnemysDistance = 1.5f;   // distance between enemies
    public float EnemysDistStrength = 2f; // how much pushback if they get to close to one anothe

    private float currentAngle; 
    private float orbitRadius;

    void Start()
    {
        if (!player) return; //add player!

        // At start we are getting playerr and each enemy distane and storing that offset to the enemy's orbitting radius
        Vector2 offset = transform.position - player.position;
        orbitRadius = offset.magnitude;
        currentAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        if (!player) return;

        // this enemy orbiting speed Based off how far the enemy is, they move based off multiplier
        float orbitSpeed = orbitRadius * speedMultip;

     
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;

        // So if the player moves this will help keep that Ideal positioning from the playerr.
        float angleRad = currentAngle * Mathf.Deg2Rad; //got the current angle and translating that into radians
        Vector3 orbitTarget = player.position + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * orbitRadius;
        //using the radians we use this so the player can find the spot to move towards too

        // the collider thing was vague, so this is just so enemies avoid each other
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, EnemysDistance);
        foreach (var other in others)
        {
            if (other != null && other.gameObject != gameObject && other.CompareTag("Enemy")) //ignore self only looking for other enemies with Tag enemy
            {
                Vector2 away = (Vector2)(transform.position - other.transform.position); //get distance between enemies
                float dist = away.magnitude;
                if (dist > 0.01f && dist < EnemysDistance) // if distance really close
                {
                    Vector2 repulsion = away.normalized * (EnemysDistStrength / dist); //normalize that distance then get the pushback
                    orbitTarget += (Vector3)(repulsion * Time.deltaTime); //adjust to new position and orbittinng distance
                }
            }
        }

        // This just adds some Lag to the enemy movement when playe moves so that it isn't too instant
        transform.position = Vector3.Lerp(transform.position, orbitTarget, EnemyLag * Time.deltaTime);

        // using (-90 degrees) we can assume that is the Up of the enemy to always face the playerr in orbit
        Vector3 faceDir = player.position - transform.position;
        float faceAngle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, faceAngle - 90f);
    }
}