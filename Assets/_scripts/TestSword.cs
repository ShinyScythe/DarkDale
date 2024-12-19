using PurrNet;
using PurrNet.Packing;
using UnityEngine;

public class TestSword : MonoBehaviour
{
    // call animator
    public NetworkAnimator animator;
    // set weapon parameters
    public float attackRange = 3f;
    public float attackTime = 1f;
    public float attackSpeed = 1f;
    public float attackDelay = 2f;
    public float damage = 5f;
    

    public LayerMask attackLayer;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;


    private void Start()
    {
        animator = GetComponentInParent<NetworkAnimator>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Attack();
        }
    }
    public void Attack()
    {
        if (!readyToAttack || attacking)
        {
            return;
        }
        readyToAttack = false;
        attacking = true;

        animator.SetBool("isAttacking", true);

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);


        // play sound
        //audioSource.pitch = Random.Range(0.9f, 1.1f)
        //audioSource.PlayOneShot(swordSwing)
    }
    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
        animator.SetBool("isAttacking", false);
    }

    void AttackRaycast()
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, attackRange, attackLayer))
        {
            HitTarget(hit.point);
        }
    }

    void HitTarget(Vector3 pos)
    {
        // visual and audio effects
    }
}
