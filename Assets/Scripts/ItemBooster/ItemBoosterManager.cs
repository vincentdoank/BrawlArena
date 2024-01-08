using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBoosterManager : MonoBehaviour
{
    private List<Booster> boosterList = new List<Booster>();

    public static ItemBoosterManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        boosterList = new List<Booster>();
        boosterList.Add(new Booster
        {
            boosterId = "SPEED",
            buffs = new List<Buff>
            {
                new Buff
                {
                    level = 100,
                    data = new BuffData
                    {
                        targetAtrMultiplier = "SPEED",
                        atrMultiplier = 0.1f,
                        atrMutiplierGrowth = 0.1f,
                        atrMultiplierGrowthLevelInterval = 10,
                        duration = 10,
                        durationGrowth = 1f,
                        durationGrowthLevelInterval = 1,
                    }
                }
            }
        });
    }

    public Booster GetRandomBooster()
    {
        int rand = Random.Range(0, boosterList.Count);
        return boosterList[rand];
    }

    public Booster GetBooster(string boosterId)
    {
        return boosterList.Where(x => x.boosterId == boosterId).FirstOrDefault();
    }
}
