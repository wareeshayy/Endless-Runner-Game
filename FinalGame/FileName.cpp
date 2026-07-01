#include<iostream>
#include <conio.h>
#include <windows.h>
#include<string>
#include<ctime>

#define ROW 30
#define COL 45

using namespace std;

char arr[ROW][COL];

char zill[3][3] = { ' ','O',' ','\\','|','/','/',' ','\\' };
//    O 
//   \|/
//   / \

char barrier1[3][7] = { '_','_', '_', '_', '_', '_','_', '|', ' ', ' ', ' ', ' ',' ', '|', '|', ' ', ' ', ' ', ' ',' ', '|' };
//  _______
//  |     |
//  |     |
char barrier2[3][7] = { '_','_', '_', '_', '_', '_','_','|', '*', '*', '*', '*','*', '|', '|', '*', '*', '*', '*','*', '|' };
//  _______
//  |*****|
//  |*****|

int zillRIndex[3] = { 26,27,28 };
int zillCIndex[3] = { 20,21,22 };
int bar1RIndex[3] = { -1,-1,-1 };
int bar1CIndex[7] = { -1,-1,-1 ,-1,-1,-1,-1 };
int bar2RIndex[3] = { -1,-1,-1 };
int bar2CIndex[7] = { -1,-1,-1 ,-1,-1,-1,-1 };

int lifelines = 3, score = 0;
char heart = 3;
bool bar1, bar2;
string name;

void display()
{
    int zrow = 0, zcol = 0, b1row = 0, b1col = 0, b2row = 0, b2col = 0, zind = 0, b1ind = 0, b2ind = 0;
    for (int i = 0; i < ROW; i++)
    {
        for (int j = 0; j < COL; j++)
        {
            if (i == ROW - 1 || i == 0)
                cout << '-';
            else if (i == zillRIndex[zrow] && j == zillCIndex[zcol])
            {
                cout << zill[zrow][zcol];
                zind++;
                zrow = zind / 3;
                zcol = zind % 3;
            }
            else if (i == bar1RIndex[b1row] && j == bar1CIndex[b1col])
            {
                cout << barrier1[b1row][b1col];
                b1ind++;
                b1row = b1ind / 7;
                b1col = b1ind % 7;
            }
            else if (i == bar2RIndex[b2row] && j == bar2CIndex[b2col])
            {
                cout << barrier2[b2row][b2col];
                b2ind++;
                b2row = b2ind / 7;
                b2col = b2ind % 7;
            }
            else if (j == COL - 1 || j == 0)
                cout << '|';
            else
                cout << ' ';
        }
        if (i == 0)
        {
            cout << "PLAYER          :   " << name;
        }
        else if (i == 1)
        {
            cout << "LIFE LINES      :   ";
            for (int i = 0; i < lifelines; i++)
            {
                cout << heart << ' ';
            }
        }
        else if (i == 2)
        {
            cout << "SCORE           :   " << score;
        }
        cout << endl;
    }

}

//barriors ko gaib krna k lia jb wo last row sa mil jain os k lia -1 krna hai to wo out of arr ho jay ga
void bar1Reset()
{
    for (int i = 0; i < 3; i++)
        bar1RIndex[i] = -1;
    for (int i = 0; i < 7; i++)
        bar1CIndex[i] = -1;
}

void bar2Reset()
{
    for (int i = 0; i < 3; i++)
        bar2RIndex[i] = -1;
    for (int i = 0; i < 7; i++)
        bar2CIndex[i] = -1;
}

//to remove the lifeline and to remove the barrier when collide
void checker(bool jump, bool slide)
{
    //for jump/slide barrior
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                for (int l = 0; l < 3; l++)
                {
                    if (zillRIndex[i] == bar1RIndex[j] && zillCIndex[k] == bar1CIndex[l] && jump == 0 && slide == 0)
                    {
                        bar1Reset();
                        lifelines--;
                        return;
                    }
                }
            }
        }
    }
    //for jumpbar barrior
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                for (int l = 0; l < 3; l++)
                {
                    if (zillRIndex[i] == bar2RIndex[j] && zillCIndex[k] == bar2CIndex[l] && jump == 0)
                    {
                        bar2Reset();
                        lifelines--;
                        return;
                    }
                }
            }
        }
    }
}

void jumper(int i)
{
    if (i > 4)
    {
        zillRIndex[0]--;
        zillRIndex[1]--;
        zillRIndex[2]--;
    }
    else
    {
        zillRIndex[0]++;
        zillRIndex[1]++;
        zillRIndex[2]++;
    }
}

//Turning the charcter Around
void slider()
{
    zill[0][0] = '\\';
    zill[0][1] = ' ';
    zill[0][2] = '/';
    zill[1][0] = '\\';
    zill[1][1] = '|';
    zill[1][2] = '/';
    zill[2][0] = ' ';
    zill[2][1] = 'O';
    zill[2][2] = ' ';
    //  \ /
    //  \|/
    //   O
}

//Returning to its original position
void slideReturn()
{
    zill[0][0] = ' ';
    zill[0][1] = 'O';
    zill[0][2] = ' ';
    zill[1][0] = '\\';
    zill[1][1] = '|';
    zill[1][2] = '/';
    zill[2][0] = '/';
    zill[2][1] = ' ';
    zill[2][2] = '\\';
    //   O 
    //  \|/
    //  / \

}

//Move Left and Right FUnction
void leftMove()
{
    if (zillCIndex[0] - 15 > 0)
    {
        zillCIndex[0] -= 15;
        zillCIndex[1] -= 15;
        zillCIndex[2] -= 15;
    }
}

void rightMove()
{
    if (zillCIndex[0] + 15 < COL)
    {
        zillCIndex[0] += 15;
        zillCIndex[1] += 15;
        zillCIndex[2] += 15;
    }
}

// to genarate the barrior in 3 columns according to rand function
void jbarLine(int line)
{
    if (line == 0)
    {
        //  _______
        //  |     |
        //  |     |
        //int bar1RIndex[3] = { -1,-1,-1 };
        //int bar1CIndex[6] = { -1,-1,-1 ,-1,-1,-1,-1 };
        bar1RIndex[0] = 1;
        bar1RIndex[1] = 2;
        bar1RIndex[2] = 3;
        bar1CIndex[0] = 3;
        bar1CIndex[1] = 4;
        bar1CIndex[2] = 5;
        bar1CIndex[3] = 6;
        bar1CIndex[4] = 7;
        bar1CIndex[5] = 8;
        bar1CIndex[6] = 9;
    }
    else if (line == 1)
    {
        for (int i = 0; i < 3; i++)
            bar1RIndex[i] = i + 1;
        for (int i = 0; i < 7; i++)
            bar1CIndex[i] = i + 18;
    }
    else if (line == 2)
    {
        for (int i = 0; i < 3; i++)
            bar1RIndex[i] = i + 1;
        for (int i = 0; i < 7; i++)
            bar1CIndex[i] = i + 33;
    }
}

void sbarLine(int line)        
{
    if (line == 0)
    {
        //  _______
        //  |*****|
        //  |*****|
        //int bar1RIndex[3] = { -1,-1,-1 };
        //int bar1CIndex[6] = { -1,-1,-1 ,-1,-1,-1,-1 };
        bar2RIndex[0] = 1;
        bar2RIndex[1] = 2;
        bar2RIndex[2] = 3;
        bar2CIndex[0] = 3;
        bar2CIndex[1] = 4;
        bar2CIndex[2] = 5;
        bar2CIndex[3] = 6;
        bar2CIndex[4] = 7;
        bar2CIndex[5] = 8;
        bar2CIndex[6] = 9;
    }
    else if (line == 1)
    {
        for (int i = 0; i < 3; i++)
            bar2RIndex[i] = i + 1;
        for (int i = 0; i < 7; i++)
            bar2CIndex[i] = i + 18;
    }
    else if (line == 2)
    {
        for (int i = 0; i < 3; i++)
            bar2RIndex[i] = i + 1;
        for (int i = 0; i < 7; i++)
            bar2CIndex[i] = i + 33;
    }
}         

//to move the barrior down
void jbarcont()            //to move the barriar down
{
    bar1RIndex[0]++;
    bar1RIndex[1]++;
    bar1RIndex[2]++;
}

void sbarcont()
{
    bar2RIndex[0]++;
    bar2RIndex[1]++;
    bar2RIndex[2]++;
}

void gameOver()
{
    cout << "\n\t\t**************         ************       *******        *******      **********   ";
    cout << "\n\t\t**************        **************     **** ****      **** ****     **********   ";
    cout << "\n\t\t****                  ****      ****    ***     ***    ***    ***     **           ";
    cout << "\n\t\t****                  ****      ****    ***     ***    ***    ***     **           ";
    cout << "\n\t\t****                  ****      ****    ***     ***    ***    ***     **********   ";
    cout << "\n\t\t****                  **************    ***     ***    ***    ***     **********   ";
    cout << "\n\t\t****     *********    **************    ***     ***    ***    ***     **           ";
    cout << "\n\t\t****     ***   ***    ****      ****    ***     ***    ***    ***     **           ";
    cout << "\n\t\t************** ***    ****      ****    ***      **** ***     ***     **********   ";
    cout << "\n\t\t************** ***    ****      ****    ***       ******      ***     **********   ";
    cout << "\n\n\t\t***********************************************************************************";
    cout << "\n\t\t                       " << name << "'s SCORE    :    " << score;
    cout << "\n\t\t***********************************************************************************\n";

    cout << "\n\t\t ************     ****                 ****    **********    ************** ";
    cout << "\n\t\t**************     ****               ****     **********    ************** ";
    cout << "\n\t\t****      ****      ****             ****      **            ****       *** ";
    cout << "\n\t\t****      ****       ****           ****       **            ****       *** ";
    cout << "\n\t\t****      ****        ****         ****        **********    ************** ";
    cout << "\n\t\t****      ****         ****       ****         **********    ************** ";
    cout << "\n\t\t****      ****          ****     ****          **            **** ****      ";
    cout << "\n\t\t****      ****           ****   ****           **            ****   ****    ";
    cout << "\n\t\t**************            **** ****            **********    ****     ****  ";
    cout << "\n\t\t ************              *******             **********    ****       ****\n\n\n";
}

void startDisplay()
{
    cout << "\n**********     *******      ***   *************        ";
    cout << "\n**********    **** ****     ***   **************       ";
    cout << "\n**           ***     ***    ***   ***        ****      ";
    cout << "\n**           ***     ***    ***   ***         *****    ";
    cout << "\n**********   ***     ***    ***   ***          *****   ";
    cout << "\n**********   ***     ***    ***   ***          *****   ";
    cout << "\n**           ***     ***    ***   ***         *****    ";
    cout << "\n**           ***     ***    ***   ***        ****      ";
    cout << "\n**********   ***      **** ****   **************       ";
    cout << "\n**********   ***       *******    *************        ";


    cout << "\n\t\t\t\t\t\t*****               **********        *********        *********    ";
    cout << "\n\t\t\t\t\t\t*****               **********     *******          *******         ";
    cout << "\n\t\t\t\t\t\t*****               **           ***              ***               ";
    cout << "\n\t\t\t\t\t\t*****               **           ****             ****              ";
    cout << "\n\t\t\t\t\t\t*****               **********     ******           ******          ";
    cout << "\n\t\t\t\t\t\t*****               **********       ******           ******        ";
    cout << "\n\t\t\t\t\t\t*****               **                 ******           ******      ";
    cout << "\n\t\t\t\t\t\t*****************   **               ******           ******        ";
    cout << "\n\t\t\t\t\t\t*****************   **********     ******           ******          ";
    cout << "\n\t\t\t\t\t\t*****************   **********   ******           ******            ";

    cout << "\n**************     ****          ****      *******      ***      *******      ***    **********    **************";
    cout << "\n**************     ****          ****     **** ****     ***     **** ****     ***    **********    **************";
    cout << "\n****       ***     ****          ****    ***     ***    ***    ***     ***    ***    **            ****       ***";
    cout << "\n****       ***     ****          ****    ***     ***    ***    ***     ***    ***    **            ****       ***";
    cout << "\n**************     ****          ****    ***     ***    ***    ***     ***    ***    **********    **************";
    cout << "\n**************     ****          ****    ***     ***    ***    ***     ***    ***    **********    **************";
    cout << "\n**** ****          ****          ****    ***     ***    ***    ***     ***    ***    **            **** ****     ";
    cout << "\n****   ****         ****        ****     ***     ***    ***    ***     ***    ***    **            ****   ****   ";
    cout << "\n****     ****         ************       ***      **** ****    ***      **** ****    **********    ****     **** ";
    cout << "\n****       ****          ******          ***       *******     ***       *******     **********    ****       ****";

}

void startGame()
{
    int loader = 1;
    for (int i = 0; i < 46; i++)
    {
        system("cls");
        cout << "\n\n\n\n\n\n\n\n\n\n\n";
        cout << "\t\t\t\t**********************************************\n";
        cout << "\t\t\t\t*             LOADING THE GAME               *\n";
        for (int j = 0; j < 4; j++)
        {
            cout << "\t\t\t\t";
            for (int k = 0; k < 46; k++)
            {
                if (j == 0 || k <= loader || j == 3 || k == 45)
                {
                    cout << '*';
                }
                else
                    cout << ' ';
            }
            cout << endl;
        }
        loader++;
        Sleep(50);
    }
    cout << "\t\t\t\t*                LETS PLAY                   *\n";
    cout << "\t\t\t\t**********************************************\n";
}

void menuDisplay()
{

    cout << "\n\n\n\n\n\n\n\n\n\n\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*        Enter To Select the Option          *\n";
    cout << "\t\t\t\t*        1. Start the Game                   *\n";
    cout << "\t\t\t\t*        2. Game Instructions                *\n";
    cout << "\t\t\t\t*        3. Game Credits                     *\n";
    cout << "\t\t\t\t*        4. EXIT                             *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*************ENTER CHOICE (1-4)    :     ";

}

void credits()
{
    system("CLS");
    cout << "\n\n\n\n\n\n\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*        GAME CREDITS                        *\n";
    cout << "\t\t\t\t*        22F-3441        Wareesha            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*********ENTER ANY KEY TO CONTINUE************\n";
    cout << "\t\t\t\t**********************************************\n";
    system("PAUSE");
}

void gameInstructions()
{
    cout << "\n\n\n\n\n\n\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*        INSTRUCTIONS                        *\n";
    cout << "\t\t\t\t*        1. Press W to JUMP                  *\n";
    cout << "\t\t\t\t*        2. Press S to SLide                 *\n";
    cout << "\t\t\t\t*        3. Press D to Move Right            *\n";
    cout << "\t\t\t\t*        3. Press A to Move Left             *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*********ENTER ANY KEY TO CONTINUE************\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*        BARRIERS ISTRUCTIONS                *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*     _______              _______           *\n";
    cout << "\t\t\t\t*     |     |              |*****|           *\n";
    cout << "\t\t\t\t*     |     |              |*****|           *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t*    JUMP/SLIDE           ONLY JUMP          *\n";
    cout << "\t\t\t\t*                                            *\n";
    cout << "\t\t\t\t**********************************************\n";
    cout << "\t\t\t\t*********ENTER ANY KEY TO CONTINUE************\n";
    system("PAUSE");
}

void game()

{
    srand(time(0));
    int counter1 = 15, counter2 = 25, line = 0;
    cout << "\n\n\n\n\n\n\n\n\n\n\t\t\tEnter Name of the Player  :  ";
    cin.ignore();// without this control ignores user to input his name..
    getline(cin, name);// gets input in string with spaces..
    int val, inp;
    bool jump = 0, slide = 0, jbar = 0, sbar = 0;
    while (lifelines != 0) {
        system("cls");
        if (_kbhit() && jump == 0 && slide == 0)
        {
            char key = _getch();
            if (key == 'w')
            {
                jump = 1;
                val = 8;
            }
            if (key == 's')
            {
                slide = 1;
                val = 8;
                slider();
            }
            if (key == 'a')
            {
                leftMove();
            }
            if (key == 'd')
            {
                rightMove();
            }
        }

        if (jump == 1)
        {
            jumper(val);
            val--;
            if (val == 0)
            {
                jump = 0;
            }
        }

        if (slide == 1)
        {
            val--;
            if (val == 0)
            {
                slideReturn();
                slide = 0;
            }
        }
        //jumpbar
        if (counter1 == 0)
        {
            bar1 = 1;
            counter1 = 30;
            line = rand() % 3;
            jbarLine(line);
            score += 50;
        }
        //sbar
        if (counter2 == 0)
        {
            bar2 = 2;
            counter2 = 30;
            line = rand() % 3;
            sbarLine(line);
            score += 50;
        }
        jbarcont();
        sbarcont();

        counter1--;
        counter2--;
        checker(jump, slide);
        display();
        score += 10;
        Sleep(50);
    }
    system("cls");
    gameOver();
    system("PAUSE");
    return;
}
int main()
{
    startDisplay();
    Sleep(5000);
    system("cls");

    int inp;
    do
    {
        do
        {
            system("cls");
            menuDisplay();

            cin >> inp;
        } while (inp < 1 || inp >4);

        if (inp == 1)
        {
            startGame();
            Sleep(500);
            game();
        }
        else if (inp == 2)
        {
            gameInstructions();
        }
        else if (inp == 3)
        {
            credits();
        }
    } while (inp != 4);

    system("PAUSE");
    return 0;
}
