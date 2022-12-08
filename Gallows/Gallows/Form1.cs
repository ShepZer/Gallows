using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Diagnostics;
using System.IO;

//сделать исключение: 763, 1353
namespace Gallows{
    public partial class Gallow : Form {

        public string save;//в данную строку будет записываться сохранение

        public string guessedLetters; //тут хранится строка из 1 и 0. 1 - буква отгадана, 0 - буква не отгадана
        public string timeSave; //тут хранится время в миллисекундах которое пользователь потратил на игру (до сохранения)
        public string wrongLetters;//строка для хранения букв, которые пользователь вводил до сохранение (только неверные буквы)
        public string correctLetters;//строка для хранения букв, которые пользователь вводил до сохранение (только верные буквы)
        public bool needSave = false;//флаг, который позволяет использовать данные, если пользователь хочет продолжить игру
        public bool winLose;//переименная, которая отмечает победил игрок или проиграл false - проиграл, true - победил

        public string save1;//первая ячейка сохранения

        public string save2;//вторая ячейка сохранения

        public string save3;//третья ячейка сохранения

        public Image gallowSprites;
        public Button[] pic = new Button[1];//сюда выводиться рисунок

        public int numberOfAttempts = 10; //количество попыток

        public int size_word = new int();//размер слова/текста
        public int lettersLeft = new int();//количество букв, которые осталось отгадать
        public char[] char_word = new char[100];//сюда записывается загаданное слово

        public Button[] butts = new Button[30];//кнопки для слова
        public Button[,] abcButts = new Button[3, 11];//кнопки для алфавита

        public char[,] abcChar = new char[3, 11] {
            { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й'},
            { 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф' },
            { 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' },
        };//алфавит

        public TextBox textBoxPlay = new TextBox();//сюда будет сохраняться слово, которое введет пользователь

        public TextBox textName = new TextBox();//сюда записывается имя при выходе
        public TextBox textFullname = new TextBox();//сюда записывается фамилия при выходе

        public TextBox textFullnameFinish = new TextBox();//имя при победе
        public TextBox textNameFinish = new TextBox();//фамилия при победе

        public Stopwatch timer = new Stopwatch();//таймер для игры
        /*==================================================================================================
         * =======================================МЕНЮ=========================================*/

        public Gallow() {
            InitializeComponent();
            size_word = 0;
            gallowSprites = new Bitmap("Gallow.png");

            InitMeny();
        }

        public void InitMeny(){//Создаем элементы меню
            
            StreamReader sr = new StreamReader("Saves.txt");
            save1 = sr.ReadLine();//считываем первое сохранение
            save2 = sr.ReadLine();//считываем второе сохранение
            save3 = sr.ReadLine();//считываем третье сохрнанение
            sr.Close();

            Controls.Clear();
            //----------------------------------------------------------надпись "Виселица"
            Label label = new Label();
            label.Size = new Size(500, 110);
            label.Location = new Point(550,50);
            label.Text = "Виселица";
            float currentSize = label.Font.Size;
            currentSize = 50;
            label.Font = new Font(label.Font.Name, currentSize, label.Font.Style, label.Font.Unit);
            this.Controls.Add(label);
            //----------------------------------------------------------кнопку "Продолжить"

            Button buttProceed = new Button();
            buttProceed.Size = new Size(450, 100);
            buttProceed.Location = new Point(525, 300);

            buttProceed.BackColor = Color.AliceBlue;
            buttProceed.Click += new EventHandler(OnButtonMenyPress);
            buttProceed.Font = new Font(buttProceed.Font.Name, 16, buttProceed.Font.Style, buttProceed.Font.Unit);
            this.Controls.Add(buttProceed);

            buttProceed.Text = ("Продолжить");
            //---------------------------------------------------------кнопка "Новая игра"

            Button buttPlay = new Button();
            buttPlay.Size = new Size(450, 100);
            buttPlay.Location = new Point(525, 420);

            buttPlay.BackColor = Color.AliceBlue;
            buttPlay.Click += new EventHandler(OnButtonMenyPress);
            buttPlay.Font = new Font(buttPlay.Font.Name, 16, buttPlay.Font.Style, buttPlay.Font.Unit);
            this.Controls.Add(buttPlay);

            //buttsMeny[2] = buttPlay;

            buttPlay.Text = ("Новая игра");
            //--------------------------------------------------------кнопка "Выход"

            Button buttExit = new Button();
            buttExit.Size = new Size(450, 100);
            buttExit.Location = new Point(525, 540); 

            buttExit.BackColor = Color.AliceBlue;
            buttExit.Click += new EventHandler(OnButtonMenyPress);
            buttExit.Font = new Font(buttExit.Font.Name, 16, buttExit.Font.Style, buttExit.Font.Unit);
            this.Controls.Add(buttExit);

            buttExit.Text = ("Выйти из игры");
            GC.Collect();
        }//Создаем элементы меню

        public void OnButtonMenyPress(object sender, EventArgs e){//Обработка нажатия в меню
            Button pressedButtonmeny = sender as Button;
            switch (pressedButtonmeny.Location.Y){
                case 300://-------------игрок нажал на кнопку "продолжить"
                    pressedButtonmeny.Dispose();
                    GC.Collect();
                    contMeny();
                    break;
                case 420://-------------игрок нажал на кнопку "новая игра"
                    pressedButtonmeny.Dispose();
                    GC.Collect();
                    enterWord();
                    break;
                case 540://-------------выход из игры (раньше была таблица лидеров)
                    Dispose();
                    Application.Exit();
                    break;
            }
            //MessageBox.Show(Convert.ToString(pressedButtonmeny.Location.X) + " " + Convert.ToString(pressedButtonmeny.Location.Y));
        }//Обработка нажатия в меню

        public void enterWord(){//менюшка после нажатия на "ИГРАТЬ"
            Controls.Clear();
            //---------------------------------- кнопка позволяющая вернуть в главное меню
            Button buttBack = new Button();
            buttBack.Size = new Size(150, 80);
            buttBack.Location = new Point(1300, 750);
            buttBack.Enabled = true;
            buttBack.Text = "Назад";
            buttBack.BackColor = Color.AliceBlue;
            buttBack.Font = new Font(buttBack.Font.Name, 10, buttBack.Font.Style, buttBack.Font.Unit);
            buttBack.Click += new EventHandler(entreWordFinish);

            this.Controls.Add(buttBack);

            //----------------------------------- сюда мгрок будет вводить слово
            TextBox text = new TextBox();
            text.Size = new Size(600, 100);
            text.Location = new Point(450, 100);
            text.Font = new Font(text.Font.Name, 30, text.Font.Style, text.Font.Unit);
            this.Controls.Add(text);

            textBoxPlay = text;
            //----------------------------------- кнопка для старта
            Button buttStart = new Button();
            buttStart.Size = new Size(600, 100);
            buttStart.Location = new Point(450, 600);
            buttStart.Font = new Font(buttStart.Font.Name, 20, buttStart.Font.Style, buttStart.Font.Unit);

            buttStart.BackColor = Color.AliceBlue;
            buttStart.Click += new EventHandler(entreWordFinish);
            this.Controls.Add(buttStart);

            buttStart.Text = ("Старт");
            //----------------------------------- кнопка позволяющая ввести случайное слово
            Button buttRand = new Button();
            buttRand.Size = new Size(200, 60);
            buttRand.Location = new Point(650, 200);
            buttRand.Font = new Font(buttRand.Font.Name, 10, buttRand.Font.Style, buttRand.Font.Unit);

            buttRand.BackColor = Color.AliceBlue;
            buttRand.Click += new EventHandler(entreWordFinish);
            this.Controls.Add(buttRand);

            buttRand.Text = ("Случайное слово");
            GC.Collect();
        }//менюшка после нажатия на "ИГРАТЬ"

        public void entreWordFinish(object sender, EventArgs e){//Оброботка кнопок в подменю "ИГРАТЬ"
            
            string bd;
            bool flag = false;

            Button pressedButtonmeny = sender as Button;
            switch (pressedButtonmeny.Location.Y){
                case 750:
                    InitMeny();
                    pressedButtonmeny.Dispose();
                    GC.Collect();
                    break;
                case 200://-------------случайное слово
                    randWord();
                    break;
                case 600://-------------НАЧАТЬ
                    try{
                        if (textBoxPlay.TextLength != 0){

                            StreamReader sr = new StreamReader("dictionary.txt");
                            
                            for (int i = 0; i < 51302; i++){//перебираем все слова в словаре
                                bd = sr.ReadLine();

                                if (textBoxPlay.Text == bd){//проверка введенного слова
                                    string word = textBoxPlay.Text;
                                    char_word = word.ToCharArray();
                                    size_word = word.Length;
                                    lettersLeft = size_word - 2;//уменьшаем на 2 потому что 1 и последняя буква выводятся в самом начале
                                    flag = true;
                                    Controls.Clear();
                                    pressedButtonmeny.Dispose();
                                    GC.Collect();
                                    StartGame();
                                }
                            }
                            sr.Close();
                            if (flag == false){ 
                                throw new Exception(); 
                            }
                        }
                        else{
                            throw new Exception();
                        }
                    }
                    catch(Exception) when (textBoxPlay.TextLength == 0){
                        MessageBox.Show("Введите слово для игры");
                    }
                    catch(Exception) when (flag == false){
                        MessageBox.Show("Введенное слово не существует в словар. Попробуйте другое слово или проверьте это на корректность ввода.");
                    }
                    break;
            }
            GC.Collect();
        }//Оброботка кнопок в подменю ИГРАТЬ

        private void randWord(){

            Random rnd = new Random();

            int index = rnd.Next(0, 51302);

            StreamReader sr = new StreamReader("dictionary.txt");
            string skip;

            while(index != 0){
                skip = sr.ReadLine();
                index--;
            }
            textBoxPlay.Text = sr.ReadLine();
            sr.Close();
            GC.Collect();
        }//случайное слово

        /*================================ИГРАТЬ====================================
         ===========================================================================*/

        private void StartGame(){//считываем слово
            for (int i = 1; i < size_word-1; i++){

                butts[i] = new Button();

                Button butt = new Button();
                butt.Size = new Size(80, 80);
                butt.Location = new Point((this.Size.Width - size_word * 80 ) / 2 + i * 80, 60);//(1000 - size_word * 50) / 2  - Позволяет выводить поцентру. 1000 - размер окна где нужно вывести в центр
                butt.Font = new Font(butt.Font.Name, 20, butt.Font.Style, butt.Font.Unit);

                butt.BackColor = Color.White;
                this.Controls.Add(butt);

                butts[i] = butt;

                butts[i].Enabled = false;
            }

            butts[0] = new Button();
            Button butt1 = new Button();
            butt1.Size = new Size(80, 80);
            butt1.Location = new Point((this.Size.Width - size_word * 80) / 2 + 0 * 80, 60);
            butt1.Font = new Font(butt1.Font.Name, 20, butt1.Font.Style, butt1.Font.Unit);

            butt1.BackColor = Color.White;
            this.Controls.Add(butt1);

            butts[0] = butt1;

            butts[0].Enabled = false;

            butts[size_word-1] = new Button();
            Button butt2 = new Button();
            butt2.Size = new Size(80, 80);
            butt2.Location = new Point((this.Size.Width - size_word * 80) / 2 + (size_word - 1) * 80, 60);
            butt2.Font = new Font(butt2.Font.Name, 20, butt2.Font.Style, butt2.Font.Unit);

            butt2.BackColor = Color.White;
            this.Controls.Add(butt2);

            butts[size_word - 1] = butt2;

            butts[size_word - 1].Enabled = false;

            butts[0].Text = Convert.ToString(char_word[0]);
            butts[size_word - 1].Text = Convert.ToString(char_word[size_word - 1]);
            GC.Collect();

            InitABC();
        }//считываем слово

        public void InitABC(){//создаем клавиатуру для ввода
            
            for (int i = 0; i < 3; i++){//1000 - 50*11 + 10*10
                for (int j = 0; j < 11; j++){
                    abcButts[i, j] = new Button();
                    Button abcbutt = new Button();
                    abcbutt.Size = new Size(70, 70);
                    abcbutt.Location = new Point((this.Size.Width - 880)/2 + j * 80, 500 + i * 80); //330 + i * 80
                    abcbutt.Font = new Font(abcbutt.Font.Name, 20, abcbutt.Font.Style, abcbutt.Font.Unit);

                    abcbutt.BackColor = Color.Orange;
                    abcbutt.Click += new EventHandler(OnButtonPress);
                    this.Controls.Add(abcbutt);

                    abcButts[i, j] = abcbutt;

                    abcbutt.Text = Convert.ToString(abcChar[i, j]);
                }
            }

            //------кнопка Выход в игре.......
            Button buttPause = new Button();
            buttPause.Size = new Size(100, 60);
            buttPause.Location = new Point(this.Size.Width - 130, 780);
            buttPause.Font = new Font(buttPause.Font.Name, 15, buttPause.Font.Style, buttPause.Font.Unit);

            buttPause.BackColor = Color.AliceBlue;
            buttPause.Click += new EventHandler(OnButtonPress);
            this.Controls.Add(buttPause);

            buttPause.Text = "Выход";
            //------------сюда вывожу картинку
            pic[0] = new Button();

            Button buttPic = new Button();
            buttPic.Size = new Size(300, 300);
            buttPic.Location = new Point((this.Size.Width-300)/2, 175);
            buttPic.Enabled = false;
            this.Controls.Add(buttPic);

            pic[0] = buttPic;

            if (needSave == true) {keyboardCheck(); }//метод, который отметит, какие буквы пользователь уже ввел }
            
            timer.Start();//начать отсчет времени после создания всех компонентов для игры
            GC.Collect();
        }//создаем клавиатуру для ввода

        public void OnButtonPress(object sender, EventArgs e){//оброботка нажатия на клаву в игре + кнопка выхода
            
            Button pressedButton = sender as Button;
            //MessageBox.Show(Convert.ToString((pressedButton.Location.Y - 500) / 80) + " "+ Convert.ToString((pressedButton.Location.X - 310) / 80) + "\n" + Convert.ToString(pressedButton.Location.Y) +" "+ Convert.ToString(pressedButton.Location.X));

            if (pressedButton.Location.Y == 780){//игрок нажал на кнопку "выход"
                pressedButton.Dispose();
                timer.Stop();
                pic[0].Dispose();
                isExit();
            }
            else{//игрок нажал на клавиатуру
                letterCheck((pressedButton.Location.Y - 500) / 80, (pressedButton.Location.X - 310) / 80,
                    abcChar[(pressedButton.Location.Y - 500) / 80, (pressedButton.Location.X - 310) / 80]);
                pressedButton.Dispose();//если убрать эту строку, то они будут краситься
            }
        }//оброботка нажатия на клаву в игре + кнопка выхода

        private void letterCheck(int x, int y, char let){//проверка наличия буквы в загаданном слове
            bool flag = false;
            if (numberOfAttempts == 0){
                MessageBox.Show("END!");
            }
            else{
                for (int j = 1; j < size_word - 1; j++){
                    butts[j].BackColor = Color.White;
                    if (char_word[j] == let){//есть ли буква в слове
                        flag = true;
                        abcButts[x, y].BackColor = Color.Green;
                        abcButts[x, y].Enabled = false;
                        showWord(j);
                        if (lettersLeft == 0){//все буквы отгаданы
                            timer.Stop();
                            winLose = true;

                            for(int i = 0; i < 3; i++){//удаление клавиатуры
                                for(int k = 0; k < 11; k++){
                                    abcButts[i, k].Dispose();
                                }
                            }
                            for(int i = 0; i < size_word; i++){//удаления кнопок для клавиатуры
                                butts[i].Dispose();
                            }
                            pic[0].Dispose();//удаление виселицы

                            win();
                        }
                    }
                }
                if( flag == false){
                    abcButts[x, y].BackColor = Color.Red;
                    abcButts[x, y].Enabled = false;
                    if (numberOfAttempts == 1){//попыток больше нет
                        timer.Stop();
                        winLose = false;

                        for (int i = 0; i < 3; i++){//удаление клавиатуры
                            for (int k = 0; k < 11; k++)
                            {
                                abcButts[i, k].Dispose();
                            }
                        }
                        for (int i = 0; i < size_word; i++){//удаления кнопок для клавиатуры
                            butts[i].Dispose();
                        }
                        pic[0].Dispose();//удаление виселицы

                        lose();
                    }
                    else{
                        //MessageBox.Show("У вас осталось " + numberOfAttempts + " попыток!");
                        drawGallows(numberOfAttempts);
                        numberOfAttempts--;  
                    }
                }
            }
            GC.Collect();
        }//проверка наличия буквы

        private void drawGallows(int a){//рисую виселицу

            Image part = new Bitmap(400, 400);
            Graphics grap = Graphics.FromImage(part);
            grap.DrawImage(gallowSprites, new Rectangle(0, 0, 300, 300), 200 * (10 - a), 0, 200, 200, GraphicsUnit.Pixel);
            pic[0].BackgroundImage = part;

            grap.Dispose();
            GC.Collect();
        }//рисую виселицу

        public void showWord(int t){//показывает верно выбранную букву в слове
            butts[t].Text = Convert.ToString(char_word[t]);
            butts[t].BackColor = Color.GreenYellow;
            lettersLeft--;
        }//показывает верно выбранную букву в слове

        public void win(){//есть игрок победил
            Controls.Clear();
            needSave = false;
            numberOfAttempts = 10;
            
            //----------------------------------------форма для заполнения ФИ игрока
            Label labelN = new Label();
            labelN.Size = new Size(200, 50);
            labelN.Location = new Point(400, 450);
            labelN.Text = "Имя";
            labelN.Font = new Font(labelN.Font.Name, 20, labelN.Font.Style, labelN.Font.Unit);
            this.Controls.Add(labelN);


            Label labelFn = new Label();
            labelFn.Size = new Size(200, 50);
            labelFn.Location = new Point(400, 550);
            labelFn.Text = "Фамилия";
            labelFn.Font = new Font(labelFn.Font.Name, 20, labelFn.Font.Style, labelFn.Font.Unit);
            this.Controls.Add(labelFn);

            TextBox textN = new TextBox();//поле для ввода имени
            textN.Size = new Size(300, 150);
            textN.Location = new Point(600, 450);
            this.Controls.Add(textN);
            textN.Font = new Font(textN.Font.Name, 20, textN.Font.Style, textN.Font.Unit);
            textNameFinish = textN;

            TextBox textFn = new TextBox();//поле для ввода фамилии
            textFn.Size = new Size(300, 150);
            textFn.Location = new Point(600, 550);
            this.Controls.Add(textFn);
            textFn.Font = new Font(textFn.Font.Name, 20, textFn.Font.Style, textFn.Font.Unit);
            textFullnameFinish = textFn;
            //-------------------------вывожу кубок в форму
            pic[0] = new Button();

            Button buttPicWin = new Button();
            buttPicWin.Size = new Size(400, 400);
            buttPicWin.Location = new Point(550, 20);
            buttPicWin.Enabled = false;
            this.Controls.Add(buttPicWin);

            pic[0] = buttPicWin;

            Image part = new Bitmap(400, 400);
            Graphics grap = Graphics.FromImage(part);
            
            grap.DrawImage(gallowSprites, new Rectangle(0, 0, 400, 400), 10, 210, 109, 94, GraphicsUnit.Pixel);
            pic[0].BackgroundImage = part;

            //-----------------кнопка "Выход из игры"
            Button buttExit = new Button();
            buttExit.Size = new Size(400, 100);
            buttExit.Location = new Point(550, 700);

            buttExit.BackColor = Color.AliceBlue;
            buttExit.Click += new EventHandler(OnButtonWinLosePress);
            buttExit.Font = new Font(buttExit.Font.Name, 20, buttExit.Font.Style, buttExit.Font.Unit);
            this.Controls.Add(buttExit);

            buttExit.Text = ("Выход из игры");
            GC.Collect();
        }//игрок победил: вывод кубка + сохранение результатов

        public void lose(){//Игрок проиграл
            Controls.Clear();
            numberOfAttempts = 10;

            needSave = false;
            //-----------------------------------для ввода ФИ
            Label labelN = new Label();
            labelN.Size = new Size(200, 50);
            labelN.Location = new Point(400, 450);
            labelN.Text = "Имя";
            labelN.Font = new Font(labelN.Font.Name, 20, labelN.Font.Style, labelN.Font.Unit);
            this.Controls.Add(labelN);


            Label labelFn = new Label();
            labelFn.Size = new Size(200, 50);
            labelFn.Location = new Point(400, 550);
            labelFn.Text = "Фамилия";
            labelFn.Font = new Font(labelFn.Font.Name, 20, labelFn.Font.Style, labelFn.Font.Unit);
            this.Controls.Add(labelFn);

            TextBox textN = new TextBox();//поле для ввода имени
            textN.Size = new Size(300, 150);
            textN.Location = new Point(600, 450);
            this.Controls.Add(textN);
            textN.Font = new Font(textN.Font.Name, 20, textN.Font.Style, textN.Font.Unit);
            textNameFinish = textN;

            TextBox textFn = new TextBox();//поле для ввода фамилии
            textFn.Size = new Size(300, 150);
            textFn.Location = new Point(600, 550);
            this.Controls.Add(textFn);
            textFn.Font = new Font(textFn.Font.Name, 20, textFn.Font.Style, textFn.Font.Unit);
            textFullnameFinish = textFn;
            //----------------------------
            pic[0] = new Button();

            Button buttPicLose = new Button();
            buttPicLose.Size = new Size(400, 400);
            buttPicLose.Location = new Point(550, 20);
            buttPicLose.Enabled = false;
            this.Controls.Add(buttPicLose);

            pic[0] = buttPicLose;

            Image part = new Bitmap(400, 400);
            Graphics grap = Graphics.FromImage(part);

            grap.DrawImage(gallowSprites, new Rectangle(0, 0, 400, 400), 0, 320, 80, 75, GraphicsUnit.Pixel);
            pic[0].BackgroundImage = part;

            //--------------------------кнопка "выход"
            Button buttExit = new Button();
            buttExit.Size = new Size(400, 100);
            buttExit.Location = new Point(550, 700);

            buttExit.BackColor = Color.AliceBlue;
            buttExit.Click += new EventHandler(OnButtonWinLosePress);
            buttExit.Font = new Font(buttExit.Font.Name, 20, buttExit.Font.Style, buttExit.Font.Unit);
            this.Controls.Add(buttExit);

            buttExit.Text = ("Выход из игры");
            GC.Collect();
        }//игрок проиграл: вывод гробика + кнопки

        public void OnButtonWinLosePress(object sender, EventArgs e){//Обработка нажатия в конце игры (победа)
            Button pressedButtonmeny = sender as Button;
            switch (pressedButtonmeny.Location.Y){
                case 700://-------------выход
                    if (textNameFinish.TextLength != 0 && textFullnameFinish.TextLength != 0) {
                        saveScore();
                        pic[0].Dispose();
                        Application.Exit();
                    }
                    else{
                        try{
                            if (textNameFinish.TextLength == 0 & textFullnameFinish.TextLength == 0){
                                throw new Exception();
                            }
                            else{
                                if (textNameFinish.TextLength == 0) { throw new Exception(); }
                                else { throw new Exception(); }
                            }
                        }
                        catch(Exception) when (textNameFinish.TextLength == 0 & textFullnameFinish.TextLength == 0){
                            MessageBox.Show("Введите Имя и Фамилию");
                        }
                        catch(Exception) when (textNameFinish.TextLength == 0){
                            MessageBox.Show("Введите Имя");
                        }
                        catch(Exception) when (textFullnameFinish.TextLength == 0){
                            MessageBox.Show("Введите Фамилию");
                        }
                    }
                    break;
            }
            GC.Collect();
        }//Обработка нажатия в конце игры (победа)

        public void saveScore(){//сохраняю результат игрока

            string path = @"Score.txt";
            FileStream file = new FileStream(path, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.End);
            StreamWriter stream = new StreamWriter(file);

            //формат записи:имя_фамилия_слово_угадал или нет_время_количество попыток

            save = Convert.ToString(textNameFinish.Text);
            save += " ";
            save += Convert.ToString(textFullnameFinish.Text);

            //слово
            save += ", Загаданное слово: ";
            save += Convert.ToString(char_word[0]);
            for (int i = 1; i < size_word; i++){
                save += Convert.ToString(char_word[i]);
            }

            //угадал или нет
            save += ", Победил? ";

            if(winLose == true) { 
                save += "Да, "; 
            }
            else {
                save += "Нет, ";
            }
            
            save += " время игры: ";
            if(needSave == true){
                int time_save = Convert.ToInt32(timeSave) + Convert.ToInt32(timer.ElapsedMilliseconds);
                if(time_save / (1000*60*60) > 0){//часы
                    save += Convert.ToString(time_save / (1000 * 60 * 60)) + "ч.";
                    time_save = time_save % (1000 * 60 * 60);
                }
                if(time_save / (1000*60) > 0){//минуты
                    save += Convert.ToString(time_save / (1000 * 60)) + "м.";
                    time_save = time_save % (1000 * 60);
                }
                if (time_save / (1000) > 0){//секунды
                    save += Convert.ToString(time_save / (1000)) + "с.";
                    time_save = time_save % (1000);
                }
                save += Convert.ToString(time_save) + "мс.";
            }
            else{
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);
                if (time / (1000 * 60 * 60) > 0){//часы
                    save += Convert.ToString(time / (1000 * 60 * 60)) + "ч.";
                    time = time % (1000 * 60 * 60);
                }
                if (time / (1000 * 60) > 0){//минуты
                    save += Convert.ToString(time / (1000 * 60)) + "м.";
                    time = time % (1000 * 60);
                }
                if (time / (1000) > 0){//секунды
                    save += Convert.ToString(time / (1000)) + "с.";
                    time = time % (1000);
                }
                save += Convert.ToString(time) + "мс.";
            }
            //количество попыток
            save += ", количество использованных попыток: ";
            save += numberOfAttempts;

            stream.WriteLine(save);
            stream.Close();
            file.Close();
            GC.Collect();
        }//сохранение счета для таблицы лидеров

        /*=====================================================Сохранение игры и выход (если не доиграл)*/

        public void isExit(){//Если игрок решит выйти из игры не завершив ее
            Controls.Clear();
            //---введите ФИ

            Label labelN = new Label();
            labelN.Size = new Size(380, 40);
            labelN.Location = new Point(250, 500);
            labelN.Text = "Введите название сохранения";
            labelN.Font = new Font(labelN.Font.Name, 15, labelN.Font.Style, labelN.Font.Unit);
            this.Controls.Add(labelN);

            TextBox textN = new TextBox();
            textN.Size = new Size(300, 800);
            textN.Location = new Point(650, 500);
            textN.Font = new Font(textN.Font.Name, 15, textN.Font.Style, textN.Font.Unit);
            this.Controls.Add(textN);
            textName = textN;
            //-----рисую
            pic[0] = new Button();

            Button buttPicWin = new Button();
            buttPicWin.Size = new Size(500, 400);
            buttPicWin.Location = new Point(500, 30);
            buttPicWin.Enabled = false;
            this.Controls.Add(buttPicWin);

            pic[0] = buttPicWin;

            Image part = new Bitmap(500, 500);
            Graphics grap = Graphics.FromImage(part);

            grap.DrawImage(gallowSprites, new Rectangle(0, 0, 500, 500), 258, 195, 165, 198, GraphicsUnit.Pixel);
            pic[0].BackgroundImage = part;

            //--------------------кнопка "Сохранить и выйти"
            Button buttSaveExit = new Button();
            buttSaveExit.Size = new Size(400, 80);
            buttSaveExit.Location = new Point(275, 700);

            buttSaveExit.BackColor = Color.CadetBlue;
            buttSaveExit.Click += new EventHandler(OnButtonPressExit);
            buttSaveExit.Font = new Font(buttSaveExit.Font.Name, 20, buttSaveExit.Font.Style, buttSaveExit.Font.Unit);
            this.Controls.Add(buttSaveExit);

            buttSaveExit.Text = ("Сохранить и выйти");
            //--------------------кнопка "Выйти"

            Button buttExit = new Button();
            buttExit.Size = new Size(400, 80);
            buttExit.Location = new Point(825, 700);

            buttExit.BackColor = Color.AliceBlue;
            buttExit.Click += new EventHandler(OnButtonPressExit);
            buttExit.Font = new Font(buttExit.Font.Name, 20, buttExit.Font.Style, buttExit.Font.Unit);
            this.Controls.Add(buttExit);

            buttExit.Text = ("Выйти");
            GC.Collect();
        }//Если игрок решит выйти из игры. Нужно предложить сохранить

        public void OnButtonPressExit(object sender, EventArgs e){//обработка нажатия в меню выхода из игры (незавершенная игра)
            Button pressedButton = sender as Button;
            string testNormalName;
            bool testName = false;
            switch (pressedButton.Location.X){
                case 275://кнопка "сохранить и выйти"
                        if (textName.TextLength != 0){
                            GC.Collect();
                            testNormalName = Convert.ToString(textName.Text);
                            for (int i = 0; i < textName.TextLength; i++){
                                if(testNormalName[i] == '/'){
                                    testName = true;
                                    MessageBox.Show("Такого имени не существует. Тупица!");//пользователь хочет ввести 
                                    break;
                                }
                            }
                            if(testName == false) { 
                                SaveGame(); 
                            }
                            //testName = false; 
                        }
                        break;
                case 825://кнопка "выйти"
                    Application.Exit();
                    numberOfAttempts = 10;
                    GC.Collect();
                    InitMeny();
                    break;
            }
        }//оброботка нажатия на кнопки (сохранить и выйти или выйти)

        public void SaveGame(){//сохранение игры

            //------------------- ник игрока
            save = Convert.ToString(textName.Text);
            //------------------- сохраняем слово
            save += "/";
            save += Convert.ToString(char_word[0]);
            for (int i = 1; i < size_word; i++){
                save += Convert.ToString(char_word[i]);
            }
            //------------------- сохраняем размер слова
            save += "/";
            save += size_word;
            //------------------- сохраняем какие буквы угадали, а какие нет 1 - угадали, 0 - нет
            save += "/";
            for (int i = 0; i < size_word; i++){
                if (butts[i].Text != Convert.ToString(char_word[i])){
                    save += "0";
                }
                else{
                    save += "1";
                }
            }
            //------------------- сохранить время игры
            save += "/";
            save += Convert.ToString(timer.ElapsedMilliseconds);
            //------------------- количество ост. попыток
            save += "/";
            if(numberOfAttempts == 10){
                save += "-";
            }
            else{
                save += numberOfAttempts;
            }
            //------------------ отгаданные буквы
            save += "/";
            for (int i = 0; i < 3; i++){
                for (int j = 0; j < 11; j++){
                    if (abcButts[i, j].BackColor == Color.Green){
                        save += abcChar[i, j];
                    }
                }
            }

            //------------------- не отгаданные буквы
            save += "/";
            for (int i = 0; i < 3; i++){
                for (int j = 0; j < 11; j++){
                    if (abcButts[i, j].BackColor == Color.Red){
                        save += abcChar[i, j];
                    }
                }
            }
            
            //выбор места сохранение
            if (save1.Length != 0 && save2.Length != 0 && save3.Length != 0){//если все ячейки сохр. заняты нужно чтобы пользователь выбрал какую перезаписать
                Controls.Clear();

                Label label1 = new Label();
                label1.Size = new Size(300, 70);
                label1.Location = new Point(250, 250);
                label1.Font = new Font(label1.Font.Name, 25, label1.Font.Style, label1.Font.Unit);
                label1.Text = "Ячейка 1:";
                this.Controls.Add(label1);

                Label label2 = new Label();
                label2.Size = new Size(300, 70);
                label2.Location = new Point(250, 450);
                label2.Font = new Font(label2.Font.Name, 25, label2.Font.Style, label2.Font.Unit);
                label2.Text = "Ячейка 2:";
                this.Controls.Add(label2);

                Label label3 = new Label();
                label3.Size = new Size(300, 70);
                label3.Location = new Point(250, 650);
                label3.Font = new Font(label2.Font.Name, 25, label2.Font.Style, label2.Font.Unit);
                label3.Text = "Ячейка 3:";
                this.Controls.Add(label3);

                Label labelSave = new Label();
                labelSave.Size = new Size(1500, 100);
                labelSave.Location = new Point(0, 40);
                labelSave.BackColor = Color.MediumOrchid;
                labelSave.TextAlign = ContentAlignment.TopCenter;
                labelSave.Font = new Font(labelSave.Font.Name, 25, labelSave.Font.Style, labelSave.Font.Unit);
                labelSave.Text = "Выберите сохранение, которое хотите перезаписать";
                this.Controls.Add(labelSave);

                //-----ячейки сохранений

                Button buttSave1 = new Button();
                buttSave1.Size = new Size(400, 150);
                buttSave1.Location = new Point(550, 200);
                buttSave1.Enabled = true;
                buttSave1.BackColor = Color.AliceBlue;
                for (int i = 0; save1[i] != '/'; i++){
                    buttSave1.Text += save1[i];
                    buttSave1.Font = new Font(buttSave1.Font.Name, 20, buttSave1.Font.Style, buttSave1.Font.Unit);
                }
                buttSave1.Click += new EventHandler(OnButtonoverwriteSaveSelect);
                this.Controls.Add(buttSave1);

                Button buttSave2 = new Button();
                buttSave2.Size = new Size(400, 150);
                buttSave2.Location = new Point(550, 400);
                buttSave2.Enabled = true;
                buttSave2.BackColor = Color.AliceBlue;
                for (int i = 0; save2[i] != '/'; i++){ 
                    buttSave2.Text += save2[i];
                    buttSave2.Font = new Font(buttSave2.Font.Name, 20, buttSave2.Font.Style, buttSave2.Font.Unit);
                }
                buttSave2.Click += new EventHandler(OnButtonoverwriteSaveSelect);
                this.Controls.Add(buttSave2);

                Button buttSave3 = new Button();
                buttSave3.Size = new Size(400, 150);
                buttSave3.Location = new Point(550, 600);
                buttSave3.Enabled = true;
                buttSave3.BackColor = Color.AliceBlue;
                for (int i = 0; save3[i] != '/'; i++){
                    buttSave3.Text += save3[i];
                    buttSave3.Font = new Font(buttSave3.Font.Name, 20, buttSave3.Font.Style, buttSave3.Font.Unit);
                }
                buttSave3.Click += new EventHandler(OnButtonoverwriteSaveSelect);
                this.Controls.Add(buttSave3);
            }
            else{
                if(save1.Length == 0){//сохраняю в 1 ячейку сохранения если она свободка
                    StreamWriter sr1 = new StreamWriter(@"Saves.txt", false);
                    //Console.WriteLine("\n!!!");
                    sr1.Close();

                    string path1 = @"Saves.txt";
                    FileStream file1 = new FileStream(path1, FileMode.OpenOrCreate);
                    file1.Seek(0, SeekOrigin.End);
                    StreamWriter stream1 = new StreamWriter(file1);

                    stream1.WriteLine(save);
                    stream1.WriteLine(save2);
                    stream1.WriteLine(save3);

                    stream1.Close();
                    file1.Close();
                    Application.Exit();
                    //InitMeny();
                }
                else{
                    if (save2.Length == 0){//сохраняю во 2 ячейку сохранения если она свободка
                        StreamWriter sr2 = new StreamWriter(@"Saves.txt", false);
                        //Console.WriteLine("\n!!!");
                        sr2.Close();

                        string path2 = @"Saves.txt";
                        FileStream file2 = new FileStream(path2, FileMode.OpenOrCreate);
                        file2.Seek(0, SeekOrigin.End);
                        StreamWriter stream1 = new StreamWriter(file2);

                        stream1.WriteLine(save1);
                        stream1.WriteLine(save);
                        stream1.WriteLine(save3);

                        stream1.Close();
                        file2.Close();
                        Application.Exit();
                        //InitMeny();
                    }
                    else {//сохраняю в 3 ячейку сохранения если она свободка
                        StreamWriter sr3 = new StreamWriter(@"Saves.txt", false);
                        //Console.WriteLine("\n!!!");
                        sr3.Close();

                        string path3 = @"Saves.txt";
                        FileStream file3 = new FileStream(path3, FileMode.OpenOrCreate);
                        file3.Seek(0, SeekOrigin.End);
                        StreamWriter stream3 = new StreamWriter(file3);

                        stream3.WriteLine(save1);
                        stream3.WriteLine(save2);
                        stream3.WriteLine(save);

                        stream3.Close();
                        file3.Close();
                        Application.Exit();
                        //InitMeny();
                    }
                }
            }
            GC.Collect();
        }//сохраняет игру.

        public void OnButtonoverwriteSaveSelect(object sender, EventArgs e){//обработка нажатия на кнопку при выборе сохранения, которое пользователь хочет перезаписать
            Button pressedButton = sender as Button;
            //MessageBox.Show(Convert.ToString(pressedButton.Location.X) + " " + Convert.ToString(pressedButton.Location.Y));
            switch (pressedButton.Location.Y){
                case 200://перезапись первой ячейки
                    StreamWriter sr1 = new StreamWriter(@"Saves.txt", false);
                    Console.WriteLine("\n!!!");
                    sr1.Close();

                    string path1 = @"Saves.txt";
                    FileStream file1 = new FileStream(path1, FileMode.OpenOrCreate);
                    file1.Seek(0, SeekOrigin.End);
                    StreamWriter stream1 = new StreamWriter(file1);

                    stream1.WriteLine(save);
                    stream1.WriteLine(save2);
                    stream1.WriteLine(save3);

                    stream1.Close();
                    file1.Close();
                    Application.Exit();
                    //InitMeny();
                    break;
                case 400://перезапись второй ячейки
                    StreamWriter sr2 = new StreamWriter(@"Saves.txt", false);
                    Console.WriteLine("\n!!!");
                    sr2.Close();

                    string path2 = @"Saves.txt";
                    FileStream file2 = new FileStream(path2, FileMode.OpenOrCreate);
                    file2.Seek(0, SeekOrigin.End);
                    StreamWriter stream2 = new StreamWriter(file2);

                    stream2.WriteLine(save1);
                    stream2.WriteLine(save);
                    stream2.WriteLine(save3);

                    stream2.Close();
                    file2.Close();
                    Application.Exit();
                    //InitMeny();
                    break;
                case 600://перезапись третьей ячейки
                    StreamWriter sr3 = new StreamWriter(@"Saves.txt", false);
                    Console.WriteLine("\n!!!");
                    sr3.Close();

                    string path3 = @"Saves.txt";
                    FileStream file3 = new FileStream(path3, FileMode.OpenOrCreate);
                    file3.Seek(0, SeekOrigin.End);
                    StreamWriter stream3 = new StreamWriter(file3);

                    stream3.WriteLine(save1);
                    stream3.WriteLine(save2);
                    stream3.WriteLine(save);

                    stream3.Close();
                    file3.Close();
                    Application.Exit();
                    //InitMeny();
                    break;
            }
        }//оброботываю какое сохранение хочет переписать пользователь

        //========================================Продолжить=================================

        public void contMeny() {//создаем меню для выбора сохранения
            Controls.Clear();

            Label label1 = new Label();
            label1.Size = new Size(300, 70);
            label1.Location = new Point(250, 250);
            label1.Font = new Font(label1.Font.Name, 25, label1.Font.Style, label1.Font.Unit);
            label1.Text = "Ячейка 1:";
            this.Controls.Add(label1);

            Label label2 = new Label();
            label2.Size = new Size(300, 70);
            label2.Location = new Point(250, 450);
            label2.Font = new Font(label2.Font.Name, 25, label2.Font.Style, label2.Font.Unit);
            label2.Text = "Ячейка 2:";
            this.Controls.Add(label2);

            Label label3 = new Label();
            label3.Size = new Size(300, 70);
            label3.Location = new Point(250, 650);
            label3.Font = new Font(label2.Font.Name, 25, label2.Font.Style, label2.Font.Unit);
            label3.Text = "Ячейка 3:";
            this.Controls.Add(label3);

            Label labelSave = new Label();
            labelSave.Size = new Size(1500, 100);
            labelSave.Location = new Point(0, 40);
            labelSave.BackColor = Color.MediumOrchid;
            labelSave.TextAlign = ContentAlignment.TopCenter;
            labelSave.Font = new Font(labelSave.Font.Name, 25, labelSave.Font.Style, labelSave.Font.Unit);
            labelSave.Text = "Сохранения";
            this.Controls.Add(labelSave);

            Button buttSave1 = new Button();
            buttSave1.Size = new Size(400, 150);
            buttSave1.Location = new Point(550, 200);
            buttSave1.BackColor = Color.AliceBlue;
            if (save1.Length != 0) {//если первая ячейка не пустая деактивируем ее
                buttSave1.Enabled = true;
                for (int i = 0; save1[i] != '/'; i++) {
                    buttSave1.Text += save1[i];//записываем Имя в кнопку
                    buttSave1.Font = new Font(buttSave1.Font.Name, 20, buttSave1.Font.Style, buttSave1.Font.Unit);
                }
                buttSave1.Click += new EventHandler(OnButtonContinue);
            }
            else {
                buttSave1.Enabled = false;
            }
            this.Controls.Add(buttSave1);

            Button buttSave2 = new Button();
            buttSave2.Size = new Size(400, 150);
            buttSave2.Location = new Point(550, 400);
            buttSave2.BackColor = Color.AliceBlue;
            if (save2.Length != 0) {//если вторая ячейка не пустая деактивируем ее
                buttSave2.Enabled = true;
                for (int i = 0; save2[i] != '/'; i++) {
                    buttSave2.Text += save2[i];//записываем Имя в кнопку
                    buttSave2.Font = new Font(buttSave2.Font.Name, 20, buttSave2.Font.Style, buttSave2.Font.Unit);
                }
                buttSave2.Click += new EventHandler(OnButtonContinue);
            }
            else {
                buttSave2.Enabled = false;
            }
            this.Controls.Add(buttSave2);

            Button buttSave3 = new Button();
            buttSave3.Size = new Size(400, 150);
            buttSave3.Location = new Point(550, 600);
            buttSave3.BackColor = Color.AliceBlue;
            if (save3.Length != 0) {//если третья ячейка не пустая деактивируем ее
                buttSave3.Enabled = true;
                for (int i = 0; save3[i] != '/'; i++){
                    buttSave3.Text += save3[i];//записываем Имя в кнопку
                    buttSave3.Font = new Font(buttSave3.Font.Name, 20, buttSave3.Font.Style, buttSave3.Font.Unit);
                }
                buttSave3.Click += new EventHandler(OnButtonContinue);
            }
            else{
                buttSave3.Enabled = false;
            }
            this.Controls.Add(buttSave3);

            Button buttBack = new Button();//кнопка "Назад"
            buttBack.Size = new Size(200, 80);
            buttBack.Location = new Point(1250, 750);
            buttBack.BackColor = Color.AliceBlue;
            buttBack.Enabled = true;
            buttBack.Font = new Font(buttBack.Font.Name, 16, buttBack.Font.Style, buttBack.Font.Unit);
            buttBack.Text = "Назад";
            buttBack.Click += new EventHandler(OnButtonContinue);
            
            this.Controls.Add(buttBack);
            GC.Collect();
        }//выбор сохраняения

        public void OnButtonContinue(object sender, EventArgs e){
            Button pressedButton = sender as Button;
            //даня/университет/11/10000000001/00:00:00.6752044/10/
            int flag1 = 0;//флаг для того чтобы пропустить Имя (оно в сохранении идет первым)
            int j = 0;//для записи имя
            timeSave = null;//время игры
            switch (pressedButton.Location.Y){
                case 750://кнопка назад
                    Controls.Clear();
                    GC.Collect();
                    InitMeny();
                    break;
                case 200://первая ячейка сохранения
                    needSave = true;
                    for (int i = 0; i != save1.Length; i++){
                        switch (flag1) {
                            case 0://пропускаем имя
                                if(save1[i] == '/') { flag1 = 1; }
                                break;
                            case 1://считываем слово
                                if(save1[i] != '/') {
                                    char_word[j] = save1[i];
                                    j++;
                                }
                                else {
                                    flag1 = 2;
                                }
                                break;
                            case 2://размер слова
                                if (save1[i] == '/'){
                                    for (int k = 0; char_word[k] != '\0'; k++){
                                        size_word++;
                                    }
                                    flag1 = 3;
                                }
                                break;
                            case 3://какие угаданы а какие нет
                                if(save1[i] != '/'){
                                    guessedLetters += save1[i];
                                }
                                else{
                                    flag1 = 4;
                                }
                                break;
                            case 4://время
                                if(save1[i] != '/'){
                                    timeSave += save1[i];
                                }
                                else{
                                    flag1 = 5;
                                }
                                break;
                            case 5://количество попыток
                                if (save1[i] == '/'){
                                    flag1 = 6;
                                }
                                break;
                            case 6://буквы которые верно введены
                                if(save1[i] != '/'){
                                    correctLetters += save1[i];
                                }
                                else{
                                    flag1 = 7;
                                }
                                break;
                            case 7://буквы которые неверно введены
                                wrongLetters += save1[i];
                                numberOfAttempts--;
                                break;
                        }
                    }
                    Controls.Clear();
                    GC.Collect();
                    StartGame();
                    break;
                case 400://вторая ячейка
                    needSave = true;
                    for (int i = 0; i != save2.Length; i++){
                        switch (flag1){
                            case 0://пропускаем имя
                                if (save2[i] == '/') { flag1 = 1; }
                                break;
                            case 1://считываем слово
                                if (save2[i] != '/'){
                                    char_word[j] = save2[i];
                                    j++;
                                }
                                else{
                                    flag1 = 2;
                                }
                                break;
                            case 2://размер слова
                                if (save2[i] == '/'){
                                    for (int k = 0; char_word[k] != '\0'; k++){
                                        size_word++;
                                    }
                                    flag1 = 3;
                                }
                                break;
                            case 3://какие угаданы а какие нет
                                if (save2[i] != '/'){
                                    guessedLetters += save2[i];
                                }
                                else{
                                    flag1 = 4;
                                }
                                break;
                            case 4://время
                                if (save2[i] != '/'){
                                    timeSave += save2[i];
                                }
                                else{
                                    flag1 = 5;
                                }
                                break;
                            case 5://количество попыток
                                if (save2[i] == '/'){
                                    flag1 = 6;
                                }
                                break;
                            case 6://буквы которые верно введены
                                if (save2[i] != '/'){
                                    correctLetters += save2[i];
                                }
                                else{
                                    flag1 = 7;
                                }
                                break;
                            case 7://буквы которые неверно введены
                                wrongLetters += save2[i];
                                numberOfAttempts--;
                                break;
                        }
                    }
                    Controls.Clear();
                    GC.Collect();
                    StartGame();
                    break;
                case 600://третья ячейка
                    needSave = true;
                    for (int i = 0; i != save3.Length; i++){
                        switch (flag1){
                            case 0://пропускаем имя
                                if (save3[i] == '/') { flag1 = 1; }
                                break;
                            case 1://считываем слово
                                if (save3[i] != '/'){
                                    char_word[j] = save3[i];
                                    j++;
                                }
                                else{
                                    flag1 = 2;
                                }
                                break;
                            case 2://размер слова
                                if (save3[i] == '/'){
                                    for (int k = 0; char_word[k] != '\0'; k++){
                                        size_word++;
                                    }
                                    flag1 = 3;
                                }
                                break;
                            case 3://какие угаданы а какие нет
                                if (save3[i] != '/'){
                                    guessedLetters += save3[i];
                                }
                                else{
                                    flag1 = 4;
                                }
                                break;
                            case 4://время
                                if (save3[i] != '/'){
                                    timeSave += save3[i];
                                }
                                else{
                                    flag1 = 5;
                                }
                                break;
                            case 5://количество попыток
                                if (save3[i] == '/'){
                                    flag1 = 6;
                                }
                                break;
                            case 6://буквы которые верно введены
                                if (save3[i] != '/'){
                                    correctLetters += save3[i];
                                }
                                else{
                                    flag1 = 7;
                                }
                                break;
                            case 7://буквы которые неверно введены
                                wrongLetters += save3[i];
                                numberOfAttempts--;
                                break;
                        }
                    }
                    Controls.Clear();
                    GC.Collect();
                    StartGame();
                    break;
            }
        }//оброботываю какое сохранение хочет переписать пользователь

        public void keyboardCheck(){//метод вызываемый при запуске сохранения

            try{
                lettersLeft = size_word - 2;
                if (correctLetters == null) { throw new Exception(); }
                for (int i = 1; i < size_word - 1; i++){ //вывести в слове буквы
                    for (int j = 0; j < correctLetters.Length; j++){
                        if (char_word[i] == correctLetters[j]){
                            lettersLeft--;
                            butts[i].Text = Convert.ToString(correctLetters[j]);
                        }
                    }
                }

                for (int i = 1; i < size_word - 1; i++){
                    if (butts[i].Text.Length != 0){//если в ячейке слова выведена буква тогда нужно отметить ее на клавиатуре
                        for (int j = 0; j < 3; j++){
                            for (int k = 0; k < 11; k++){
                                if (butts[i].Text == abcButts[j, k].Text){
                                    abcButts[j, k].Enabled = false;
                                    abcButts[j, k].BackColor = Color.Green;
                                    abcButts[j, k].Dispose();
                                }
                            }
                        }
                    }
                }
                
            }
            catch(Exception) {}

            try{
                if (wrongLetters == null) { throw new Exception(); }
                for (int i = 0; i < 3; i++){//отмечаем буквы, которые были введены неверно
                    for (int j = 0; j < 11; j++){
                        for (int k = 0; k < wrongLetters.Length; k++){
                            if (abcButts[i, j].Text == Convert.ToString(wrongLetters[k])){
                                abcButts[i, j].Enabled = false;
                                abcButts[i, j].BackColor = Color.Red;
                                abcButts[i, j].Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception){}
            GC.Collect();
            drawGallows(numberOfAttempts + 1);
        }//метод вызываемый при запуске сохранения
    }
}
