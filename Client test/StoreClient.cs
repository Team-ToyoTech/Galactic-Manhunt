using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_test
{
    public partial class StoreClient : Form
    {
        // textBox에 입력한 값에 가격을 곱한 값을 Value에 저장
        // Value의 총합 = 지불해야 할 크로노 총합

        enum Item
        {
            hydrogen,
            nitrogen,
            oxygen,
            epsilonCrystal,
            peroxide,
            hydrazine,
            epsilon,
            water,
            food,
            seed
        }

        enum CopAbility
        {
            darkUnderTheLamp,
            galaxyTravel,
            planetTravel,
            handcuff,
            teamIdentify,
            storageGrowth
        }

        enum ThiefAbility
        {
            getFuel,
            fuelChanger,
            fuelCompressor,
            stunRemover,
            storageGrowth
        }

        // 아이템 양에 따른 크로노 양
        private Dictionary<Item, double> itemPrice
            = new Dictionary<Item, double>
            {
                { Item.hydrogen, 0 },
                { Item.nitrogen, 0 },
                { Item.oxygen, 0 },
                { Item.epsilonCrystal, 0 },
                { Item.peroxide, 0 },
                { Item.hydrazine, 0 },
                { Item.epsilon, 0 },
                { Item.water, 0 },
                { Item.food, 0 },
                { Item.seed, 0 }
            };

        // 경찰 능력에 따른 크로노 양
        private Dictionary<CopAbility, int> copAbilityPrice
            = new Dictionary<CopAbility, int>
            {
                { CopAbility.darkUnderTheLamp, 0 },
                { CopAbility.galaxyTravel, 0 },
                { CopAbility.planetTravel, 0 },
                { CopAbility.handcuff, 0 },
                { CopAbility.teamIdentify, 0 },
                { CopAbility.storageGrowth, 0 }
            };

        // 도둑 능력에 따른 크로노 양
        private Dictionary<ThiefAbility, int> thiefAbilityPrice
            = new Dictionary<ThiefAbility, int>
            {
                { ThiefAbility.getFuel, 0 },
                { ThiefAbility.fuelChanger, 0 },
                { ThiefAbility.fuelCompressor, 0 },
                { ThiefAbility.stunRemover, 0 },
                { ThiefAbility.storageGrowth, 0 }
            };

        TaskSelection form;
        ChatClient chatClient;

        public StoreClient(TaskSelection form, ChatClient chatClient)
        {
            InitializeComponent();
            this.form = form;
            this.chatClient = chatClient;
        }

        private void StoreClient_Load(object sender, EventArgs e)
        {
            // 아이템 목록
            // 이름, 가격, 남은 양
            dataGridView1.Rows.Add("수소", 400, 0);
            dataGridView1.Rows.Add("질소", 120, 0);
            dataGridView1.Rows.Add("산소", 100, 0);
            dataGridView1.Rows.Add("엑실론-크리스탈", 1000, 0);
            dataGridView1.Rows.Add("퍼옥사이드", 1440, 0);
            dataGridView1.Rows.Add("하이드라진", 1488, 0);
            dataGridView1.Rows.Add("엑실론", 2400, 0);
            dataGridView1.Rows.Add("물", 100, 0);
            dataGridView1.Rows.Add("식량", 600, 0);
            dataGridView1.Rows.Add("씨앗", 400, 0);

            // 경찰 능력 목록
            if (chatClient.job == Job.cop)
            {
                dataGridView2.Size = new Size(498, 232); // 경찰은 능력 6개
                button3.Size = new Size(60, 232);
                textBox15.Visible = true;
                label6.Location = new Point(13, 719);
                this.Size = new Size(727, 816);

                // 이름, 가격, 남은 양
                dataGridView2.Rows.Add("등잔 밑이 어둡다", 10000, 0);
                dataGridView2.Rows.Add("은하 탐방", 50000, 0);
                dataGridView2.Rows.Add("행성 탐방", 12000, 0);
                //dataGridView2.Rows.Add("스턴", "∞"); // 스턴은 기본 스킬로 개수 제한 없이 사용할 수 있음
                dataGridView2.Rows.Add("수갑", 2000, 0);
                dataGridView2.Rows.Add("팀 식별", 2000, 0);
                dataGridView2.Rows.Add("저장량 증가", 16000, 0);
            }

            // 도둑 능력 목록
            else if (chatClient.job == Job.robber)
            {
                dataGridView2.Size = new Size(498, 200); // 도둑은 능력 5개
                button3.Size = new Size(60, 200);
                textBox15.Visible = false;
                label6.Location = new Point(13, 687);
                this.Size = new Size(727, 784);

                // 이름, 가격, 남은 양
                dataGridView2.Rows.Add("겟 퓨얼", 5000, 0);
                dataGridView2.Rows.Add("연료 교환권", 5000, 0);
                dataGridView2.Rows.Add("연료 압축기", 10000, 0);
                dataGridView2.Rows.Add("스턴 제거기", 5000, 0);
                dataGridView2.Rows.Add("저장량 증가", 16000, 0);
            }

            // TODO: 아이템, 능력의 기능 설명해주는 ToolTip 추가

            dataGridView1.ClearSelection(); // 셀 선택 해제
            dataGridView2.ClearSelection();

            ShowTotalItemPrice(); // 초기 가격 0 표시
        }

        private void StoreClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: TaskSelection 본인 턴 확인 후 실행
            //if (본인 턴)
            //{
            TaskSelection taskSelection = new TaskSelection(chatClient, false);
            taskSelection.Show();
            //}
            //else
            //{
            //    TaskSelection taskSelection = new TaskSelection(chatClient, true);
            //    taskSelection.Show();
            //}
        }

        // 아이템 가격 총합 표시
        private void ShowTotalItemPrice()
        {
            double totalPrice = 0;

            // 아이템 가격 총합
            foreach (var price in itemPrice)
            {
                totalPrice += price.Value;
            }
            label5.Text = "합계 금액: " + totalPrice + " Cr";
        }

        private void ShowTotalAbilityPrice()
        {
            double totalPrice = 0;

            if(chatClient.job == Job.cop)
            {
                // 경찰 능력 가격 총합
                foreach (var price in copAbilityPrice)
                {
                    totalPrice += price.Value;
                }
            }
            else if (chatClient.job == Job.robber)
            {
                // 도둑 능력 가격 총합
                foreach (var price in thiefAbilityPrice)
                {
                    totalPrice += price.Value;
                }
            }

            label6.Text = "합계 금액: " + totalPrice + " Cr";
        }

        #region item_textBox_TextChanged
        private void textBox1_TextChanged(object sender, EventArgs e) // 수소
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "0";
            }
            double mass = Convert.ToDouble(textBox1.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox1.Text = dataGridView1.Rows[0].Cells[2].Value.ToString();
            }

            itemPrice[Item.hydrogen] = mass * 400;
            ShowTotalItemPrice();
        }

        private void textBox2_TextChanged(object sender, EventArgs e) // 질소
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "0";
            }
            double mass = Convert.ToDouble(textBox2.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox2.Text = dataGridView1.Rows[1].Cells[2].Value.ToString();
            }

            itemPrice[Item.nitrogen] = mass * 120;
            ShowTotalItemPrice();
        }

        private void textBox4_TextChanged(object sender, EventArgs e) // 산소 
        {
            if (textBox4.Text == "")
            {
                textBox4.Text = "0";
            }
            double mass = Convert.ToDouble(textBox4.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox4.Text = dataGridView1.Rows[2].Cells[2].Value.ToString();
            }

            itemPrice[Item.oxygen] = mass * 100;
            ShowTotalItemPrice();
        }

        private void textBox3_TextChanged(object sender, EventArgs e) // 엑실론-크리스탈
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "0";
            }
            double mass = Convert.ToDouble(textBox3.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox3.Text = dataGridView1.Rows[3].Cells[2].Value.ToString();
            }

            itemPrice[Item.epsilonCrystal] = mass * 1000;
            ShowTotalItemPrice();
        }

        private void textBox8_TextChanged(object sender, EventArgs e) // 퍼옥사이드
        {
            if (textBox8.Text == "")
            {
                textBox8.Text = "0";
            }
            double mass = Convert.ToDouble(textBox8.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[4].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox8.Text = dataGridView1.Rows[4].Cells[2].Value.ToString();
            }

            itemPrice[Item.peroxide] = mass * 0; // TODO: 가격 정하기
            ShowTotalItemPrice();
        }

        private void textBox7_TextChanged(object sender, EventArgs e) // 하이드라진
        {
            if (textBox7.Text == "")
            {
                textBox7.Text = "0";
            }
            double mass = Convert.ToDouble(textBox7.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[5].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox7.Text = dataGridView1.Rows[5].Cells[2].Value.ToString();
            }

            itemPrice[Item.hydrazine] = mass * 0; // TODO: 가격 정하기
            ShowTotalItemPrice();
        }

        private void textBox6_TextChanged(object sender, EventArgs e) // 엑실론
        {
            if (textBox6.Text == "")
            {
                textBox6.Text = "0";
            }
            double mass = Convert.ToDouble(textBox6.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[6].Cells[2].Value);
            if (mass > maxinum)  // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox6.Text = dataGridView1.Rows[6].Cells[2].Value.ToString();
            }

            itemPrice[Item.epsilon] = mass * 0; // TODO: 가격 정하기
            ShowTotalItemPrice();
        }

        private void textBox5_TextChanged(object sender, EventArgs e) // 물
        {
            if (textBox5.Text == "")
            {
                textBox5.Text = "0";
            }
            double mass = Convert.ToDouble(textBox5.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[7].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox5.Text = dataGridView1.Rows[7].Cells[2].Value.ToString();
            }

            itemPrice[Item.water] = mass * 100;
            ShowTotalItemPrice();
        }

        private void textBox10_TextChanged(object sender, EventArgs e) // 식량
        {
            if (textBox10.Text == "")
            {
                textBox10.Text = "0";
            }
            double mass = Convert.ToDouble(textBox10.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[8].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox10.Text = dataGridView1.Rows[8].Cells[2].Value.ToString();
            }

            itemPrice[Item.food] = mass * 600;
            ShowTotalItemPrice();
        }

        private void textBox9_TextChanged(object sender, EventArgs e) // 씨앗
        {
            if (textBox9.Text == "")
            {
                textBox9.Text = "0";
            }
            double mass = Convert.ToDouble(textBox9.Text);
            double maxinum = Convert.ToDouble(dataGridView1.Rows[9].Cells[2].Value);
            if (mass > maxinum) // mass가 maxinum보다 크면 maxinum을 mass로
            {
                mass = maxinum;
                textBox9.Text = dataGridView1.Rows[9].Cells[2].Value.ToString();
            }

            itemPrice[Item.seed] = mass * 400;
            ShowTotalItemPrice();
        }
        #endregion

        #region ability_textBox_TextChanged
        // 아래는 모두 경찰 기준
        // 도둑은 역할을 받아오는 것을 구현하면 if문으로 구분하여 처리
        // 도둑 능력 처리할 때 편하라고 이름 주석은 함수 않으로 넣음

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            if (textBox20.Text == "")
            {
                textBox20.Text = "0";
            }
            int num = Convert.ToInt32(textBox20.Text);
            int maxinum = Convert.ToInt32(dataGridView2.Rows[0].Cells[2].Value);
            if (num > maxinum) // num이 maxinum보다 크면 maxinum을 num으로
            {
                num = maxinum;
                textBox20.Text = dataGridView2.Rows[0].Cells[2].Value.ToString();
            }

            if (chatClient.job == Job.cop)
            {
                // 등잔 밑이 어둡다
                copAbilityPrice[CopAbility.darkUnderTheLamp] = num * 10000;
            }
            else if (chatClient.job == Job.robber)
            {
                // 겟 퓨얼
                thiefAbilityPrice[ThiefAbility.getFuel] = num * 5000;
            }

            ShowTotalAbilityPrice();
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            if (textBox19.Text == "")
            {
                textBox19.Text = "0";
            }
            int num = Convert.ToInt32(textBox19.Text);
            int maxinum = Convert.ToInt32(dataGridView2.Rows[1].Cells[2].Value);
            if (num > maxinum) // num이 maxinum보다 크면 maxinum을 num으로
            {
                num = maxinum;
                textBox19.Text = dataGridView2.Rows[1].Cells[2].Value.ToString();
            }

            if (chatClient.job == Job.cop)
            {
                // 은하 탐방
                copAbilityPrice[CopAbility.galaxyTravel] = num * 50000;
            }
            else if (chatClient.job == Job.robber)
            {
                // 연료 교환권
                thiefAbilityPrice[ThiefAbility.fuelChanger] = num * 5000;
            }

            ShowTotalAbilityPrice();
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            if (textBox18.Text == "")
            {
                textBox18.Text = "0";
            }
            int num = Convert.ToInt32(textBox18.Text);
            int maxinum = Convert.ToInt32(dataGridView2.Rows[2].Cells[2].Value);
            if (num > maxinum) // num이 maxinum보다 크면 maxinum을 num으로
            {
                num = maxinum;
                textBox18.Text = dataGridView2.Rows[2].Cells[2].Value.ToString();
            }

            if (chatClient.job == Job.cop)
            {
                // 행성 탐방
                copAbilityPrice[CopAbility.planetTravel] = num * 12000;
            }
            else if (chatClient.job == Job.robber)
            {
                // 연료 압축기
                thiefAbilityPrice[ThiefAbility.fuelCompressor] = num * 10000;
            }

            ShowTotalAbilityPrice();
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            if (textBox17.Text == "")
            {
                textBox17.Text = "0";
            }
            int num = Convert.ToInt32(textBox17.Text);
            int maxinum = Convert.ToInt32(dataGridView2.Rows[3].Cells[2].Value);
            if (num > maxinum) // num이 maximun보다 크면 maximun을 num으로
            {
                num = maxinum;
                textBox17.Text = dataGridView2.Rows[3].Cells[2].Value.ToString();
            }

            if (chatClient.job == Job.cop)
            {
                // 수갑
                copAbilityPrice[CopAbility.handcuff] = num * 2000;
            }
            else if (chatClient.job == Job.robber)
            {
                // 스턴 제거기
                thiefAbilityPrice[ThiefAbility.stunRemover] = num * 5000;
            }

            ShowTotalAbilityPrice();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            if (textBox16.Text == "")
            {
                textBox16.Text = "0";
            }
            int num = Convert.ToInt32(textBox16.Text);
            int maxinum = Convert.ToInt32(dataGridView2.Rows[4].Cells[2].Value);
            if (num > maxinum) // num이 maxinum보다 크면 maxinum을 num으로
            {
                num = maxinum;
                textBox16.Text = dataGridView2.Rows[4].Cells[2].Value.ToString();
            }

            if (chatClient.job == Job.cop)
            {
                // 팀 식별
                copAbilityPrice[CopAbility.teamIdentify] = num * 2000;
            }
            else if (chatClient.job == Job.robber)
            {
                // 저장량 증가
                thiefAbilityPrice[ThiefAbility.storageGrowth] = num * 16000;
            }

            ShowTotalAbilityPrice();
        }

        private void textBox15_TextChanged(object sender, EventArgs e) // 도둑은 능력이 5개이므로 도둑인 경우 visible = false
        {
            if (textBox15.Text == "")
            {
                textBox15.Text = "0";
            }
            // 저장량 증가
            int num = Convert.ToInt32(textBox15.Text);
            int maxinum = Convert.ToInt32(dataGridView2.Rows[5].Cells[2].Value);
            if (num > maxinum) // num이 maxinum보다 크면 maxinum을 num으로
            {
                num = maxinum;
                textBox15.Text = dataGridView2.Rows[5].Cells[2].Value.ToString();
            }

            copAbilityPrice[CopAbility.storageGrowth] = num * 16000;
            ShowTotalAbilityPrice();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e) // 아이템 구매
        {
            DialogResult = MessageBox.Show("아이템을 구매하시겠습니까?", "구매 확인", MessageBoxButtons.YesNo);
            if (DialogResult == DialogResult.Yes)
            {
                // 아이템 구매
                MessageBox.Show("구매 완료"); // 구매 완료를 표현하는 방법은 이거 말고 label로 표현해주세요, 지금은 확인용으로 임시입니다
            }
            string msg = textBox1.Text + ","
                + textBox2.Text + ","
                + textBox4.Text + ","
                + textBox3.Text + ","
                + textBox8.Text + ","
                + textBox7.Text + ","
                + textBox6.Text + ","
                + textBox5.Text + ","
                + textBox10.Text + ","
                + textBox9.Text;
            chatClient.Send("200", msg);
        }

        #region itemKeyDown
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = MessageBox.Show("능력을 구매하시겠습니까?", "구매 확인", MessageBoxButtons.YesNo);
            if (DialogResult == DialogResult.Yes)
            {
                // 아이템 구매
                MessageBox.Show("구매 완료"); // 구매 완료를 표현하는 방법은 이거 말고 label로 표현해주세요, 지금은 확인용으로 임시입니다
            }
            string msg = textBox20.Text + ","
                + textBox19.Text + ","
                + textBox18.Text + ","
                + textBox17.Text + ","
                + textBox16.Text + ","
                + textBox15.Text;
            chatClient.Send("300", msg);
        }

        #region abilityKeyDown
        private void textBox20_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }

        private void textBox19_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }

        private void textBox18_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }

        private void textBox17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }

        private void textBox16_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }

        private void textBox15_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }
        #endregion
    }
}