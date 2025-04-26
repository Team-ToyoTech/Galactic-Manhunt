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

    // 선원 클래스
    class Sailor
    {
        SailorType type; // 선원 타입
        public double eatInDay; // 식사량
        public bool isDisabled; // True: 행동 불능, False: 정상
        public Inventory inventory;
        // public Work work;

        /*public Sailor(SailorType T, double EatINDAY, double Maxinventory, Work work)
        {
            inventory = new Inventory(Maxinventory, 0);
            type = T;
            eatInDay = EatINDAY;
            isDisabled = false;
            this.work = work;
        }*/

        public Sailor(SailorType type, double eatInDay, double maxInventory)
        {
            inventory = new Inventory(maxInventory, 0);
            this.type = type;
            this.eatInDay = eatInDay;
            isDisabled = false;
        }

        public SailorType sailorType
        {
            get { return type; } // TODO: 한글로 반환 필요
        }
    }

    // 선원 타입
    enum SailorType
    {
        normal,
        advanced
    }
}