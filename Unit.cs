using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { Sword, Range, Guard, Wizard};
public enum Condition { Move, Attack, Hit, Die, Stop};

public class Unit : MonoBehaviour
{
	public UnitType type; //������ Ÿ��
	public int speed; 
	public ParticleSystem Dust;

	private float distance; //������ ����
	LayerMask enemy; //������ ���̾�
	LayerMask ally; //�Ʊ��� ���̾�
	RaycastHit2D hit; 
	RaycastHit2D wait;
	Animator anim;
	Vector3 direction; //���� Ž�� �������� �� ����
	Vector3 allyDirection; // �Ʊ� Ž�� �������� �� ����
	private Condition con; //������ �� ����
	private bool enemyScan = true;
	BoxCollider2D boxCol;
	SpriteRenderer ren;
	GameObject Arrow;
	ParticleSystem Masic;

	float Hp;
	float Attack;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		boxCol = GetComponent<BoxCollider2D>();
		ren = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		//���� Ÿ�� ���� �ʿ��� �� ����
		TypeSet();
	}

	private void FixedUpdate()
	{
		doMove();
	}

	private void doMove()
	{
		if (con == Condition.Move) //�̵� ������ �� ����Ʈ ��ƼŬ ���� �� �̵�
		{
			if (enemyScan) // ���� Ž�� ���� true �϶�
			{
				hit = Physics2D.Raycast(transform.position, direction, distance, enemy); //���� Ž���ϴ� ������ 
				Debug.DrawRay(transform.position, direction, Color.green, 0f);
				if (hit)
				{
					con = Condition.Attack; //���� Attack���� �ٲ�
					doAttack(); //����
				}
				else // Ž�� ���н� ���� Ž�� false
					enemyScan = false;
			}
			else //���� Ž�� false�� ��
			{
				wait = Physics2D.Raycast(transform.position, allyDirection, 0.3f, ally);//�Ʊ� Ž�� �������� ���� Ÿ�Կ� ���� ��ȭ���� ����
				Debug.DrawRay(transform.position, allyDirection, Color.blue, 0f);
				if (wait)
				{
					con = Condition.Stop;//Ž�� �Ǹ� Stop����
					doStop();
				}
				else //�Ʊ� Ž�� ���н� ���� Ž�� true
					enemyScan = true;
			}
			if (!(Dust == null))
				if(!Dust.isPlaying)
					Dust.Play();
			transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, 0f);// �̵�
		}
		else//�̵� ���� �ƴҶ� ��ƼŬ ���� ����
		{
			if (!(Dust == null)) //null�̶� .isPlaying üũ�ؾ� ���׾���
				if(Dust.isPlaying)
					Dust.Stop();
		}
	}

	private void TypeSet()
	{
		//�� ������ ���� ���ϴ� �� ����
		if (gameObject.layer == 8)
		{
			ally = LayerMask.GetMask("Blue");
			enemy = LayerMask.GetMask("Red");
			Arrow = GameManager.instance.ArrowBlue;
			Masic = GameManager.instance.MasicBlue;
			allyDirection = Vector3.right * 0.3f;
		}
		else if (gameObject.layer == 9)
		{
			ally = LayerMask.GetMask("Red");
			enemy = LayerMask.GetMask("Blue");
			Arrow = GameManager.instance.ArrowRed;
			Masic = GameManager.instance.MasicRed;
			allyDirection = Vector3.left * 0.3f;
		}

		switch (type) // ���� Ÿ�Ժ��� ���� Ž�� ������ ���� ����
		{
			case UnitType.Sword:
				Hp = 10f;
				Attack = 2f;
				distance = 0.7f;
				break;
			case UnitType.Range:
				Hp = 10f;
				Attack = 1f;
				distance = 2.5f;
				Arrow.GetComponent<Bullet>().damage = Attack;
				break;
			case UnitType.Guard:
				Hp = 30f;
				Attack = 1f;
				distance = 0.7f;
				break;
			case UnitType.Wizard:
				Hp = 10f;
				Attack = 1f;
				distance = 3.5f;
				break;
		}

		//���� Ÿ���� ������ �����ؾ��ϴ� ��
		if (gameObject.layer == 8)
		{
			direction = Vector3.right * distance;
		}
		else if (gameObject.layer == 9)
		{
			direction = Vector3.left * distance;
		}



		// ù ����� �̵����� ����
		con = Condition.Move;
		anim.SetBool("doMove", true);
	}

	private void doAttack()
	{
		if (con == Condition.Attack)
		{
			anim.SetBool("doAttack", true);
			StartCoroutine(AttackDelay());
		}
	}

	IEnumerator AttackDelay()
	{
		if (type == UnitType.Wizard) //Ŭ���� wizard�� ��� ���̴��� �ɸ� ����� ��ġ�� ��ƼŬ ���
			Instantiate(Masic, hit.transform.position, Quaternion.identity);
		if (type == UnitType.Range) // ���ݽ� ������ ������ ȭ�� ����
			Instantiate(Arrow, new Vector3(transform.position.x, 0.3f, 0), Quaternion.identity);
		else
			hit.collider.GetComponent<Unit>().TakeDamage(Attack); //������ �ֱ�, �������� ��� ȭ�쿡 ���� TakeDamage�� ȣ���ϱ� ������ �������ش�.
		yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
		con = Condition.Move;
		anim.SetBool("doMove", true);
	}

	private void doStop()
	{
		if (con == Condition.Stop)
		{
			StartCoroutine(Delay(2f));
		}

	}
	IEnumerator Delay(float delay)
	{
		yield return new WaitForSeconds(delay);
		con = Condition.Move;
		anim.SetBool("doMove", true);
	}

	public void TakeDamage(float Attack)
	{
		StopAllCoroutines(); // �ǰ� �� �̹� �������� �ٸ� �����̵�� ���ļ� ���װ� ���� �ʵ��� ��� �ڷ�ƾ�� �����ϰ� �����Ѵ�.
		Hp -= Attack;
		if (Hp > 0)
		{
			con = Condition.Hit;
			anim.SetBool("doHit", true);
			StartCoroutine(Delay(0.8f));
		}
		else
		{
			con = Condition.Die;
			anim.SetBool("doDie", true);
			boxCol.enabled = false;
			ren.sortingOrder = 1;
			Destroy(gameObject,3f); //this�� �ϴϱ� ��ũ��Ʈ�� ������ ������Ʈ�� �����־���....
		}
	}
}
