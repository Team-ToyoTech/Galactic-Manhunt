using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_test
{
    /*                     ⚠️ 중 요 ⚠️                     */
    /* !! 수정 후 Client에 있는 파일도 똑같이 수정해줘야 함 !! */
    /* !!      바꿀 때 namespace Server_test 로 변경      !! */

    // 인벤토리 클래스
    public class Inventory
    {
        List<Item> items;                    // 아이템 저장
        Dictionary<Ability, int> abilities;  // 능력 저장,    이름, 개수
        double itemMax;                      // 아이템 최댓값, 단위: kg
        int abilityMax;                      // 능력 최댓값, 단위: 개
        int inventoryLevel = 0;             // 인벤토리 레벨ㄴ

        public Inventory(double itemMax, int abilityMax) 
        {
            items = new List<Item>();
            abilities = new Dictionary<Ability, int>();
            this.itemMax = itemMax;
            this.abilityMax = abilityMax;
        }

        // 아이템 최대량 반환
        public double ItemMax
        {
            get { return itemMax; }
        }

        // 능력 최대량 반환
        public int AbilityMax
        { 
            get { return abilityMax; }
        }

        // 아이템 리스트 반환
        public List<Item> Items
        {
            get { return items; }
        }

        // 능력 리스트 반환
        public Dictionary<Ability, int> Abilities
        { 
            get { return abilities; } 
        }

        // 아이템 최대량 설정
        public void SetItemMax(double itemMax)
        {
            this.itemMax = itemMax;
        }

        // 능력 최대량 설정
        public void SetAbilityMax(int abilityMax)
        {
            this.abilityMax = abilityMax;
        }

        // 아이템 추가
        public void AddItem(Item item)
        {
            foreach(var i in items)
            {
                if (i.GetItemType() == item.GetItemType())
                {
                    i.mass += item.mass;
                    return;
                }
            }
            items.Add(item);
        }

        // 아이템 삭제
        public bool RemoveItem(Item item) // if문 사용해서 빠졌는지 안빠졌는지 꼭 체크해줘야함
        {
            foreach (var i in items)
            {
                if (i.GetItemType() == item.GetItemType())
                {
                    if (i.mass - item.mass >= 0)
                    {
                        i.mass -= item.mass;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        // 아이템 초기화(전체 삭제)
        public void ItemClear()
        {
            items.Clear();
        }


        // 능력 추가
        public void AddAbility(Ability ability)
        {
            foreach (var ab in abilities)
            {
                if (ability.GetType() == ab.GetType())
                {
                    abilities[ab.Key]++;
                }
            }
        }

        // 능력 삭제 __ 구현은 RemoveItem과 동일한 방식
        public bool RemoveAbility(Ability ability)
        {
            foreach (var Ab in abilities)
            {
                if (ability.GetType() == ability.GetType())
                {
                    if (abilities[Ab.Key] - 1 >= 0)
                    {
                        abilities[Ab.Key]--;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        // 능력 초기화
        public void AbilityClear()
        {
            abilities.Clear();
        }

        // 두 인벤토리 더하기
        public static Inventory operator +(Inventory inv, Item item)
        {
            foreach (var i in inv.Items)
            {
                if (i.GetItemType() == item.GetItemType())
                {
                    i.mass += item.mass;
                    return inv;
                }
            }
            inv.items.Add(item);
            return inv;
        }
    }
}