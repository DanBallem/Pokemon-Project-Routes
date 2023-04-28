using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource healSound;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource backgroundMusic;

    public GameObject Pidgey;
    public GameObject Rattata;

    public GameObject playerPrefab;
    private GameObject enemyPrefab;

    private int encounterDigit;

    public string sceneToLoad;

    public Transform spawnPoint;

    private string textContents;
    public string damageText;
    public string superDamageText;
    public string notDamageText;

    private Unit playerUnit;
    private Unit enemyUnit;

    public TextMeshProUGUI dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    private int playerDamageValue;
    private int enemyDamageValue;

    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        RandomEncounterGenerator();
        backgroundMusic.Play();
        state = BattleState.START;
        StartCoroutine(SetUpBattle());

        Debug.Log(playerDamageValue);
        Debug.Log(enemyDamageValue);
    }
    private void RandomEncounterGenerator()
    {
        encounterDigit = Random.Range(1, 10);
        if (encounterDigit <= 6)
        {
            enemyPrefab = Pidgey;
        }
        else
        {
            enemyPrefab = Rattata;
        }
    }
}
    IEnumerator SetUpBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, spawnPoint);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A Wild " + enemyUnit.unitName + " Appeared!";

        playerHUD.setHUD(playerUnit);
        enemyHUD.setHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        if (enemyDamageValue == enemyUnit.damage)
        {
            textContents = damageText;
        }
        if (enemyDamageValue > enemyUnit.damage)
        {
            textContents = notDamageText;
        }
        if (enemyDamageValue < enemyUnit.damage)
        {
            textContents = superDamageText;
        }
        bool isDead = enemyUnit.TakeDamage(playerDamageValue);

        enemyHUD.setHP(enemyUnit.currentHP);
        dialogueText.text = "The Attack Hits!";
        yield return new WaitForSeconds(1f);
        dialogueText.text = textContents;

        yield return new WaitForSeconds(2f);

        if (enemyUnit.currentHP <= 0)
        {
            EndBattle();
            state = BattleState.WON;
            backgroundMusic.Stop();
            winSound.Play();
        } else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }    
    }

    IEnumerator EnemyTurn()
    {
        if (enemyDamageValue == enemyUnit.damage)
        {
            textContents = damageText;
        }
        if (enemyDamageValue > enemyUnit.damage)
        {
            textContents = superDamageText;
        }
        if (enemyDamageValue < enemyUnit.damage)
        {
            textContents = notDamageText;
        }
        dialogueText.text = enemyUnit.unitName + " Attacked!";
        damageSound.Play();
        bool isDead = playerUnit.TakeDamage(enemyDamageValue);
        yield return new WaitForSeconds(1f);
        dialogueText.text = textContents;
        yield return new WaitForSeconds(1f);

        playerHUD.setHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        } else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator EndBattle()
    {
        dialogueText.text = "The Wild " + enemyUnit.unitName + " Fainted!";
        yield return new WaitForSeconds(2f);
        dialogueText.text = "You Won The Battle!";
        yield return new WaitForSeconds(2f);
        dialogueText.text = "Press SPACEBAR To Contiunue...";
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "What Will You Do?";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);

        playerHUD.setHP(playerUnit.currentHP);
        dialogueText.text = "Your HP has increased by 5!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
        buttonSound.Play();
        damageSound.PlayDelayed(0.5f);
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        buttonSound.Play();
        healSound.PlayDelayed(0.5f);
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

    /*void NormalModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.immuneDamage;
            enemyDamageValue = enemyUnit.immuneDamage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void FireModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void WaterModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void GrassModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void ElectricModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void IceModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
    }
    void FightingModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.immuneDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void PoisonModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void GroundModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.immuneDamage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void FlyingModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.immuneDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void PsychicModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void BugModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void RockModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void GhostModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.immuneDamage;
            enemyDamageValue = enemyUnit.immuneDamage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.immuneDamage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.notEffectiveDamage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
    }
    void DragonModifier()
    {
        if (enemyUnit.unitType == "Normal")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Fire")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Water")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Grass")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Electric")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ice")
        {
            playerDamageValue = playerUnit.notEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
        if (enemyUnit.unitType == "Fighting")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Poison")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ground")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Flying")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Psychic")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Bug")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Rock")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Ghost")
        {
            playerDamageValue = playerUnit.damage;
            enemyDamageValue = enemyUnit.damage;
        }
        if (enemyUnit.unitType == "Dragon")
        {
            playerDamageValue = playerUnit.superEffectiveDamage;
            enemyDamageValue = enemyUnit.superEffectiveDamage;
        }
    }*/
}