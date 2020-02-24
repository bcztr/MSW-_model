using ILOG.Concert;
using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication41020;

namespace ConsoleApplication41020
{
    class Program
    {
        static void Main(string[] args)
        {

            DateTime A = DateTime.Now;
            const int source = 4;//废物产生点数i
            const int TS = 6;//转运站数j
            const int recyclable = 2;//可回收垃圾处理厂数q
            const int dry = 4;//干垃圾处理厂数u
            const int kitchen = 2;//湿垃圾处理厂数a
            const int harmful = 1;//有害垃圾处理厂数v


            int[] Gib = { 900, 1600, 1300, 600 };//产生点产生的可回收垃圾数量                                                                        
            int[] Gic = { 3700, 6100, 4900, 2400 };//产生点产生的干垃圾数量 
            int[] Gid = { 1700, 3000, 2300, 1200 };//产生点产生的湿垃圾数量 
            int[] Gie = { 0, 0, 1, 0 };//产生点产生的有害垃圾数量 


            int[] FCj = { 4600000, 4700000, 4700000, 4780000, 4760000, 4740000 };//建设转运站的固定成本
            int[] FCq = { 4300000, 4250000 };//建设可回收垃圾处理厂的固定成本 
            int[] FCu = { 4750000, 4200000, 4000000, 4500000 };//建设干垃圾处理厂的固定成本 
            int[] FCa = { 4700000, 4500000 };//建设湿垃圾处理厂的固定成本 
            int[] FCv = { 4600000 };//建设有害垃圾处理厂的固定成本


            int[] capj = { 5800, 6300, 6300, 6700, 6500, 6800 };//转运站的年最大容量
            int[] capq = { 3500, 2000 };//可回收垃圾处理厂的年最大容量
            int[] capu = { 5000, 4000, 2500, 3800 };//干垃圾处理厂的年最大容量
            int[] capa = { 4000, 5500 };//湿垃圾处理厂的年最大容量
            int[] capv = { 4000 };//有害垃圾处理厂的年最大容量


            int[] OCj = { 460, 470, 470, 478, 476, 474 };//转运站的运营成本
            int[] OCq = { 860, 8508 };//可回收垃圾处理厂的运营成本 
            int[] OCu = { 950, 840, 800, 900 };//干垃圾处理厂的运营成本 
            int[] OCa = { 1000, 950 };//湿垃圾处理厂的运营成本 
            int[] OCv = { 1300 };//有害垃圾处理厂的运营成本

            const double TCij = 2.5;//i-j中收集车辆的单位运输费用 RMB/ton*km  2
            const double TCjk = 2.4;//j-k中运输车辆的单位运输费用 RMB/ton*km 0.8

            const double fuelij = 0.2;//收集车辆的单位距离和载重能耗 kg/ton*km
            const double fueljk = 0.8;//运输车辆的单位距离和载重能耗 kg/ton*km

            const double fjb = 0.4;//转运站处理每单位可回收垃圾的碳排放 kg/ton
            const double fjc = 0.5;//转运站处理每单位干垃圾的碳排放 kg/ton
            const double fjd = 0.6;//转运站处理每单位湿垃圾的碳排放 kg/ton
            const double fje = 0.4;//转运站处理每单位有害垃圾的碳排放 kg/ton

            const double fq = 2.0;//可回收垃圾厂处理每单位可回收废物的碳排放 kg/ton
            const double fu = 2.5;//干垃圾厂处理每单位干垃圾的碳排放 kg/ton
            const double fa = 3.0;//湿垃圾厂处理每单位湿垃圾的碳排放 kg/ton
            const double fv = 4.0;//有害垃圾厂处理每单位有害废物的碳排放 kg/ton

            const double mfj = 900000;//转运站的最大碳排放 kg/ton
            const double mfq = 700000;//可回收垃圾厂的最大碳排放 kg/ton
            const double mfu = 100000;//干垃圾厂的最大碳排放 kg/ton
            const double mfa = 800000;//湿垃圾厂的最大碳排放 kg/ton
            const double mfv = 700000;//有害垃圾厂的最大碳排放 kg/ton


            const double Zo = 0.1618;//CO2排放系数  
            const double bo = 1.2;//单位CO2排放量的碳税 


            const double rateq = 0.2;//可回收垃圾厂转化率
            const double rateu = 0.8;//干垃圾厂转化率
            const double ratea = 0.8;//湿垃圾厂转化率


            const double pr1 = 0.5;//每单位产品p的售价
            const double pr2 = 0.4;//每单位产品p'的售价

            const int B = 13;//转运站可建造的最大数量
            const long M = 10000000000000000;//无穷大的正数


            double[] regionj = { 1.0, 2.0, 6.5, 6.0, 8.0, 7.0 };  //转运站j处的区域影响因素，  城市总人口密度/j处人口密度
            double[] regionq = { 1.0, 2.0 };  //可回收垃圾处理厂q处的区域影响因素，  城市总人口密度/q处人口密度
            double[] regionu = { 1.0, 2.0, 3.5, 3.0 };  //湿垃圾处理厂u处的区域影响因素，  城市总人口密度/u处人口密度
            double[] regiona = { 1.5, 2.0 };  //干垃圾处理厂a处的区域影响因素，  城市总人口密度/a处人口密度
            double[] regionv = { 2.0 };  //有害垃圾处理厂v处的区域影响因素，  城市总人口密度/v处人口密度

            int[] fjobj = { 11, 13, 13, 14, 13, 14 }; //转运站j处提供的固定工作数量
            int[] fjobq = { 7, 4 }; //可回收垃圾处理厂q处提供的固定工作数量
            int[] fjobu = { 10, 10, 10, 10 }; //湿垃圾处理厂u处提供的固定工作数量
            int[] fjoba = { 10, 10 }; //干垃圾处理厂a处提供的固定工作数量
            int[] fjobv = { 10 }; //有害垃圾处理厂v处提供的固定工作数量

            double[] mjobj = { 0.005, 0.02, 0.01, 0.02, 0.01, 0.02 }; //转运站j处最大工作能力时雇佣人数/转运站的年最大容量
            double[] mjobq = { 0.004, 0.002 }; //可回收垃圾处理厂q处最大工作能力时雇佣人数/可回收垃圾处理厂的年最大容量
            double[] mjobu = { 0.005, 0.004, 0.0025, 0.004 }; //湿垃圾处理厂u处最大工作能力时雇佣人数/湿垃圾处理厂的年最大容量
            double[] mjoba = { 0.004, 0.006 }; //干垃圾处理厂a处最大工作能力时雇佣人数/干垃圾处理厂的年最大容量
            double[] mjobv = { 0.01 }; //有害垃圾处理厂v处最大工作能力时雇佣人数/有害垃圾处理厂的年最大容量


            int[][] dij = new int[source][];
            for (int i = 0; i < source; i++)
            {
                dij[i] = new int[TS];
            }
            for (int i = 0; i < source; i++)
            {
                for (int j = 0; j < TS; j++)
                {
                    dij[0][0] = 5;
                    dij[0][1] = 5;
                    dij[0][2] = 6;
                    dij[0][3] = 4;
                    dij[0][4] = 8;
                    dij[0][5] = 3;


                    dij[1][0] = 0;
                    dij[1][1] = 3;
                    dij[1][2] = 5;
                    dij[1][3] = 10;
                    dij[1][4] = 4;
                    dij[1][5] = 3;


                    dij[2][0] = 7;
                    dij[2][1] = 8;
                    dij[2][2] = 5;
                    dij[2][3] = 6;
                    dij[2][4] = 4;
                    dij[2][5] = 8;


                    dij[3][0] = 11;
                    dij[3][1] = 12;
                    dij[3][2] = 8;
                    dij[3][3] = 10;
                    dij[3][4] = 7;
                    dij[3][5] = 12;

                }
            }//i-j距离



            int[][] djq = new int[TS][];
            for (int j = 0; j < TS; j++)
            {
                djq[j] = new int[recyclable];
            }
            for (int j = 0; j < TS; j++)
            {
                for (int q = 0; q < recyclable; q++)
                {
                    djq[0][0] = 31;
                    djq[0][1] = 67;


                    djq[1][0] = 40;
                    djq[1][1] = 67;


                    djq[2][0] = 34;
                    djq[2][1] = 69;


                    djq[3][0] = 37;
                    djq[3][1] = 79;


                    djq[4][0] = 34;
                    djq[4][1] = 68;


                    djq[5][0] = 30;
                    djq[5][1] = 74;

                }
            }//j-q距离






            int[][] dju = new int[TS][];
            for (int j = 0; j < TS; j++)
            {
                dju[j] = new int[dry];
            }
            for (int j = 0; j < TS; j++)
            {

                for (int u = 0; u < dry; u++)
                {

                    dju[0][0] = 6;
                    dju[0][1] = 4;
                    dju[0][2] = 8;
                    dju[0][3] = 3;

                    dju[1][0] = 5;
                    dju[1][1] = 10;
                    dju[1][2] = 4;
                    dju[1][3] = 3;

                    dju[2][0] = 5;
                    dju[2][1] = 6;
                    dju[2][2] = 4;
                    dju[2][3] = 8;

                    dju[3][0] = 8;
                    dju[3][1] = 10;
                    dju[3][2] = 7;
                    dju[3][3] = 12;

                    dju[4][0] = 6;
                    dju[4][1] = 8;
                    dju[4][2] = 6;
                    dju[4][3] = 3;

                    dju[5][0] = 19;
                    dju[5][1] = 28;
                    dju[5][2] = 20;
                    dju[5][3] = 21;

                }
            }//j-u距离



            int[][] dja = new int[TS][];
            for (int j = 0; j < TS; j++)
            {
                dja[j] = new int[kitchen];
            }
            for (int j = 0; j < TS; j++)
            {
                for (int a = 0; a < kitchen; a++)
                {
                    dja[0][0] = 28;
                    dja[0][1] = 76;


                    dja[1][0] = 33;
                    dja[1][1] = 108;

                    dja[2][0] = 25;
                    dja[2][1] = 73;

                    dja[3][0] = 22;
                    dja[3][1] = 89;

                    dja[4][0] = 24;
                    dja[4][1] = 72;

                    dja[5][0] = 27;
                    dja[5][1] = 78;

                }
            }//j-a距离



            int[][] djv = new int[TS][];
            for (int j = 0; j < TS; j++)
            {
                djv[j] = new int[harmful];
            }
            for (int j = 0; j < TS; j++)
            {
                for (int v = 0; v < harmful; v++)
                {

                    djv[0][0] = 26;

                    djv[1][0] = 27;

                    djv[2][0] = 29;

                    djv[3][0] = 23;

                    djv[4][0] = 27;

                    djv[5][0] = 27;

                }
            }//j-v距离









            Cplex model = new Cplex();//建立cplex模型
            //决策变量声明

            INumVar[][] Qijb = new INumVar[source][];//收集车辆从产生点i到转运站j运输可回收垃圾b运量
            INumVar[][] Qijc = new INumVar[source][];//收集车辆从产生点i到转运站j运输干垃圾c运量
            INumVar[][] Qijd = new INumVar[source][];//收集车辆从产生点i到转运站j运输湿垃圾d运量
            INumVar[][] Qije = new INumVar[source][];//收集车辆从产生点i到转运站j运输有害垃圾e运量

            for (int i = 0; i < source; i++)
            {
                Qijb[i] = new INumVar[TS];
                for (int j = 0; j < TS; j++)
                {
                    Qijb[i][j] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量i-j运输可回收垃圾b的运量


            for (int i = 0; i < source; i++)
            {
                Qijc[i] = new INumVar[TS];
                for (int j = 0; j < TS; j++)
                {
                    Qijc[i][j] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量i-j运输干垃圾c的运量


            for (int i = 0; i < source; i++)
            {
                Qijd[i] = new INumVar[TS];
                for (int j = 0; j < TS; j++)
                {
                    Qijd[i][j] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量i-j运输湿垃圾d的运量


            for (int i = 0; i < source; i++)
            {
                Qije[i] = new INumVar[TS];
                for (int j = 0; j < TS; j++)
                {
                    Qije[i][j] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量i-j运输有害垃圾e的运量



            INumVar[][] Qjqb = new INumVar[TS][];//运输车辆从转运站j到可回收垃圾处理厂q运输可回收垃圾b运量
            INumVar[][] Qjuc = new INumVar[TS][];//运输车辆从转运站j到干垃圾处理厂u运输干垃圾c运量
            INumVar[][] Qjad = new INumVar[TS][];//运输车辆从转运站j到湿垃圾处理厂a运输湿垃圾d运量
            INumVar[][] Qjve = new INumVar[TS][];//运输车辆从转运站j到有害垃圾处理厂v运输有害垃圾e运量      


            for (int j = 0; j < TS; j++)
            {
                Qjqb[j] = new INumVar[recyclable];
                for (int q = 0; q < recyclable; q++)
                {
                    Qjqb[j][q] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量j-q运输可回收垃圾b的运量


            for (int j = 0; j < TS; j++)
            {
                Qjuc[j] = new INumVar[dry];
                for (int u = 0; u < dry; u++)
                {
                    Qjuc[j][u] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量j-u运输干垃圾c的运量


            for (int j = 0; j < TS; j++)
            {
                Qjad[j] = new INumVar[kitchen];
                for (int a = 0; a < kitchen; a++)
                {
                    Qjad[j][a] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量j-a运输湿垃圾d的运量


            for (int j = 0; j < TS; j++)
            {
                Qjve[j] = new INumVar[harmful];
                for (int v = 0; v < harmful; v++)
                {
                    Qjve[j][v] = model.NumVar(0, 1000000, NumVarType.Int);
                }
            }//决策变量j-e运输有害垃圾e的运量



            INumVar[] X = new INumVar[TS];//是否建立转运站j
            INumVar[] Y = new INumVar[recyclable];//是否建立可回收垃圾处理厂q
            INumVar[] U = new INumVar[dry];//是否建立干垃圾处理厂u
            INumVar[] W = new INumVar[kitchen];//是否建立湿垃圾处理厂a
            INumVar[] V = new INumVar[harmful];//是否建立有害垃圾处理厂v


            for (int j = 0; j < TS; j++)
            {
                X[j] = model.NumVar(0, 1, NumVarType.Bool);
            }//转运站j是否开放（0,1）


            for (int q = 0; q < recyclable; q++)
            {
                Y[q] = model.NumVar(0, 1, NumVarType.Bool);
            }//可回收垃圾处理厂q是否开放（0,1）


            for (int u = 0; u < dry; u++)
            {
                U[u] = model.NumVar(0, 1, NumVarType.Bool);
            }//干垃圾处理厂是否开放（0,1）


            for (int a = 0; a < kitchen; a++)
            {
                W[a] = model.NumVar(0, 1, NumVarType.Bool);
            }//湿垃圾处理厂是否开放（0,1）

            for (int v = 0; v < harmful; v++)
            {
                V[v] = model.NumVar(0, 1, NumVarType.Bool);
            }//有害垃圾处理厂是否开放（0,1）












            //约束条件
            //将转运站、处理设施的排放限制在政府条例规定的标准之内
            //第1个约束条件
            INumExpr[] hh01 = new INumExpr[4];
            INumExpr[] hh02 = new INumExpr[4];
            for (int j = 0; j < TS; j++)
            {
                hh01[0] = Qijb[0][0];
                hh01[1] = Qijc[0][0];
                hh01[2] = Qijd[0][0];
                hh01[3] = Qije[0][0];
                for (int i = 0; i < source; i++)
                {
                    hh01[0] = model.Sum(hh01[0], Qijb[i][j]);
                    hh01[1] = model.Sum(hh01[1], Qijc[i][j]);
                    hh01[2] = model.Sum(hh01[2], Qijd[i][j]);
                    hh01[3] = model.Sum(hh01[3], Qije[i][j]);
                }
                hh01[0] = model.Sum(hh01[0], model.Prod(-1, Qijb[0][0]));
                hh01[0] = model.Prod(hh01[0], fjb);

                hh01[1] = model.Sum(hh01[1], model.Prod(-1, Qijc[0][0]));
                hh01[1] = model.Prod(hh01[1], fjc);

                hh01[2] = model.Sum(hh01[2], model.Prod(-1, Qijd[0][0]));
                hh01[2] = model.Prod(hh01[2], fjd);

                hh01[3] = model.Sum(hh01[3], model.Prod(-1, Qije[0][0]));
                hh01[3] = model.Prod(hh01[3], fje);
                hh01[0] = model.Sum(model.Sum(hh01[0], hh01[1]), model.Sum(hh01[2], hh01[3]));


                hh02[0] = Qijb[0][0];
                hh02[1] = Qijc[0][0];
                hh02[2] = Qijd[0][0];
                hh02[3] = Qije[0][0];
                for (int i = 0; i < source; i++)
                {
                    hh02[0] = model.Sum(hh02[0], model.Prod(dij[i][j], Qijb[i][j]));
                    hh02[1] = model.Sum(hh02[1], model.Prod(dij[i][j], Qijc[i][j]));
                    hh02[2] = model.Sum(hh02[2], model.Prod(dij[i][j], Qijd[i][j]));
                    hh02[3] = model.Sum(hh02[3], model.Prod(dij[i][j], Qije[i][j]));
                }
                hh02[0] = model.Sum(hh02[0], model.Prod(-1, Qijb[0][0]));
                hh02[1] = model.Sum(hh02[1], model.Prod(-1, Qijc[0][0]));
                hh02[2] = model.Sum(hh02[2], model.Prod(-1, Qijd[0][0]));
                hh02[3] = model.Sum(hh02[3], model.Prod(-1, Qije[0][0]));

                hh02[0] = model.Sum(model.Sum(hh02[0], hh02[1]), model.Sum(hh02[2], hh02[3]));
                hh02[0] = model.Prod(hh02[0], fuelij);
                model.AddLe(model.Sum(hh01[0], hh02[0]), mfj);
            } //对应于论文中的约束（7）



            //第2个约束条件
            INumExpr[] hh2 = new INumExpr[2];
            for (int q = 0; q < recyclable; q++)
            {
                hh2[0] = Qjqb[0][0];
                hh2[1] = Qjqb[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh2[0] = model.Sum(hh2[0], Qjqb[j][q]);
                }
                hh2[0] = model.Sum(hh2[0], model.Prod(-1, Qjqb[0][0]));
                hh2[0] = model.Prod(hh2[0], fq);

                for (int j = 0; j < TS; j++)
                {
                    hh2[1] = model.Sum(hh2[1], model.Prod(Qjqb[j][q], djq[j][q]));
                }
                hh2[1] = model.Sum(hh2[1], model.Prod(-1, Qjqb[0][0]));
                hh2[1] = model.Prod(hh2[1], fueljk);
                model.AddLe(model.Sum(hh2[0], hh2[1]), mfq);
            }//对应于论文中的约束（8）



            //第3个约束条件
            INumExpr[] hh3 = new INumExpr[2];
            for (int u = 0; u < dry; u++)
            {
                hh3[0] = Qjuc[0][0];
                hh3[1] = Qjuc[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh3[0] = model.Sum(hh3[0], Qjuc[j][u]);
                }
                hh3[0] = model.Sum(hh3[0], model.Prod(-1, Qjuc[0][0]));
                hh3[0] = model.Prod(hh3[0], fu);

                for (int j = 0; j < TS; j++)
                {
                    hh3[1] = model.Sum(hh3[1], model.Prod(Qjuc[j][u], dju[j][u]));
                }
                hh3[1] = model.Sum(hh3[1], model.Prod(-1, Qjuc[0][0]));
                hh3[1] = model.Prod(hh3[1], fueljk);
                model.AddLe(model.Sum(hh3[0], hh3[1]), mfu);
            }//对应于论文中的约束（9）



            //第4个约束条件
            INumExpr[] hh4 = new INumExpr[2];
            for (int a = 0; a < kitchen; a++)
            {
                hh4[0] = Qjad[0][0];
                hh4[1] = Qjad[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh4[0] = model.Sum(hh4[0], Qjad[j][a]);
                }
                hh4[0] = model.Sum(hh4[0], model.Prod(-1, Qjad[0][0]));
                hh4[0] = model.Prod(hh4[0], fa);

                for (int j = 0; j < TS; j++)
                {
                    hh4[1] = model.Sum(hh4[1], model.Prod(Qjad[j][a], dja[j][a]));
                }
                hh4[1] = model.Sum(hh4[1], model.Prod(-1, Qjad[0][0]));
                hh4[1] = model.Prod(hh4[1], fueljk);
                model.AddLe(model.Sum(hh4[0], hh4[1]), mfa);
            }//对应于论文中的约束（10）



            //第5个约束条件
            INumExpr[] hh5 = new INumExpr[2];
            for (int v = 0; v < harmful; v++)
            {
                hh5[0] = Qjve[0][0];
                hh5[1] = Qjve[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh5[0] = model.Sum(hh5[0], Qjve[j][v]);
                }
                hh5[0] = model.Sum(hh5[0], model.Prod(-1, Qjve[0][0]));
                hh5[0] = model.Prod(hh5[0], fv);

                for (int j = 0; j < TS; j++)
                {
                    hh5[1] = model.Sum(hh5[1], model.Prod(Qjve[j][v], djv[j][v]));
                }
                hh5[1] = model.Sum(hh5[1], model.Prod(-1, Qjve[0][0]));
                hh5[1] = model.Prod(hh5[1], fueljk);
                model.AddLe(model.Sum(hh5[0], hh5[1]), mfv);
            }//对应于论文中的约束（11）




            //确保废物运输到已经存在的设施
            //第6个约束条件,
            INumExpr[] hh6 = new INumExpr[4];
            for (int j = 0; j < TS; j++)
            {
                hh6[0] = Qijb[0][0];
                hh6[1] = Qijc[0][0];
                hh6[2] = Qijd[0][0];
                hh6[3] = Qije[0][0];

                for (int i = 0; i < source; i++)
                {
                    hh6[0] = model.Sum(hh6[0], Qijb[i][j]);
                    hh6[1] = model.Sum(hh6[1], Qijc[i][j]);
                    hh6[2] = model.Sum(hh6[2], Qijd[i][j]);
                    hh6[3] = model.Sum(hh6[3], Qije[i][j]);
                }
                hh6[0] = model.Sum(hh6[0], model.Prod(Qijb[0][0], -1));
                hh6[1] = model.Sum(hh6[1], model.Prod(Qijc[0][0], -1));
                hh6[2] = model.Sum(hh6[2], model.Prod(Qijd[0][0], -1));
                hh6[3] = model.Sum(hh6[3], model.Prod(Qije[0][0], -1));

                hh6[0] = model.Sum(model.Sum(hh6[0], hh6[1]), model.Sum(hh6[2], hh6[3]));

                model.AddLe(hh6[0], model.Prod(M, X[j]));
            }//对应于论文中的约束（12）


            //第7个约束条件
            for (int q = 0; q < recyclable; q++)
            {
                for (int j = 0; j < TS; j++)
                {
                    model.AddLe(Qjqb[j][q], model.Prod(Y[q], M));
                }
            }//对应于论文中的约束（13）



            //第8个约束条件
            for (int u = 0; u < dry; u++)
            {
                for (int j = 0; j < TS; j++)
                {
                    model.AddLe(Qjuc[j][u], model.Prod(U[u], M));
                }
            }//对应于论文中的约束（14）



            //第9个约束条件
            for (int a = 0; a < kitchen; a++)
            {
                for (int j = 0; j < TS; j++)
                {
                    model.AddLe(Qjad[j][a], model.Prod(W[a], M));
                }
            }//对应于论文中的约束（15）



            //第10个约束条件
            for (int v = 0; v < harmful; v++)
            {
                for (int j = 0; j < TS; j++)
                {
                    model.AddLe(Qjve[j][v], model.Prod(V[v], M));
                }
            }//对应于论文中的约束（16）





            //保证进入处理设施的流量不超过进入转运站的流量
            //第11个约束条件
            INumExpr[] hh111 = new INumExpr[4];
            INumExpr[] hh112 = new INumExpr[4];
            for (int j = 0; j < TS; j++)
            {
                hh111[0] = Qjqb[0][0];
                hh111[1] = Qjuc[0][0];
                hh111[2] = Qjad[0][0];
                hh111[3] = Qjve[0][0];

                for (int q = 0; q < recyclable; q++)
                {
                    hh111[0] = model.Sum(hh111[0], Qjqb[j][q]);
                }
                hh111[0] = model.Sum(hh111[0], model.Prod(-1, Qjqb[0][0]));

                for (int u = 0; u < dry; u++)
                {
                    hh111[1] = model.Sum(hh111[1], Qjuc[j][u]);
                }
                hh111[1] = model.Sum(hh111[1], model.Prod(-1, Qjuc[0][0]));
                hh111[1] = model.Sum(hh111[0], hh111[1]);

                for (int a = 0; a < kitchen; a++)
                {
                    hh111[2] = model.Sum(hh111[2], Qjad[j][a]);
                }
                hh111[2] = model.Sum(hh111[2], model.Prod(-1, Qjad[0][0]));
                hh111[2] = model.Sum(hh111[1], hh111[2]);

                for (int v = 0; v < harmful; v++)
                {
                    hh111[3] = model.Sum(hh111[3], Qjve[j][v]);
                }
                hh111[3] = model.Sum(hh111[3], model.Prod(-1, Qjve[0][0]));
                hh111[3] = model.Sum(hh111[2], hh111[3]);


                hh112[0] = Qijb[0][0];
                hh112[1] = Qijc[0][0];
                hh112[2] = Qijd[0][0];
                hh112[3] = Qije[0][0];
                for (int i = 0; i < source; i++)
                {
                    hh112[0] = model.Sum(hh112[0], Qijb[i][j]);
                    hh112[1] = model.Sum(hh112[1], Qijc[i][j]);
                    hh112[2] = model.Sum(hh112[2], Qijd[i][j]);
                    hh112[3] = model.Sum(hh112[3], Qije[i][j]);
                }
                hh112[0] = model.Sum(hh112[0], model.Prod(-1, Qijb[0][0]));
                hh112[1] = model.Sum(hh112[1], model.Prod(-1, Qijc[0][0]));
                hh112[1] = model.Sum(hh112[0], hh112[1]);

                hh112[2] = model.Sum(hh112[2], model.Prod(-1, Qijd[0][0]));
                hh112[2] = model.Sum(hh112[1], hh112[2]);

                hh112[3] = model.Sum(hh112[3], model.Prod(-1, Qije[0][0]));
                hh112[3] = model.Sum(hh112[2], hh112[3]);

                model.AddLe(hh111[3], hh112[3]);

            } //对应于论文中的约束（17）





            //流入转运站、处理设施的流量不能超过其现有能力
            //第12个约束条件
            INumExpr[] hh12 = new INumExpr[4];
            for (int j = 0; j < TS; j++)
            {
                hh12[0] = Qijb[0][0];
                hh12[1] = Qijc[0][0];
                hh12[2] = Qijd[0][0];
                hh12[3] = Qije[0][0];
                for (int i = 0; i < source; i++)
                {
                    hh12[0] = model.Sum(hh12[0], Qijb[i][j]);
                    hh12[1] = model.Sum(hh12[1], Qijc[i][j]);
                    hh12[2] = model.Sum(hh12[2], Qijd[i][j]);
                    hh12[3] = model.Sum(hh12[3], Qije[i][j]);
                }
                hh12[0] = model.Sum(hh12[0], model.Prod(-1, Qijb[0][0]));
                hh12[1] = model.Sum(hh12[1], model.Prod(-1, Qijc[0][0]));
                hh12[1] = model.Sum(hh12[0], hh12[1]);

                hh12[2] = model.Sum(hh12[2], model.Prod(-1, Qijd[0][0]));
                hh12[2] = model.Sum(hh12[1], hh12[2]);

                hh12[3] = model.Sum(hh12[3], model.Prod(-1, Qije[0][0]));
                hh12[3] = model.Sum(hh12[2], hh12[3]);
                model.AddLe(hh12[3], model.Prod(X[j], capj[j]));
            }//对应于论文中的约束（18）



            //第13个约束条件
            INumExpr[] hh13 = new INumExpr[recyclable];
            for (int q = 0; q < recyclable; q++)
            {
                hh13[q] = Qjqb[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh13[q] = model.Sum(hh13[q], Qjqb[j][q]);
                }
                hh13[q] = model.Sum(hh13[q], model.Prod(-1, Qjqb[0][0]));
                model.AddLe(hh13[q], model.Prod(Y[q], capq[q]));
            } //对应于论文中的约束（19）



            //第14个约束条件
            INumExpr[] hh14 = new INumExpr[dry];
            for (int u = 0; u < dry; u++)
            {
                hh14[u] = Qjuc[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh14[u] = model.Sum(hh14[u], Qjuc[j][u]);
                }
                hh14[u] = model.Sum(hh14[u], model.Prod(-1, Qjuc[0][0]));
                model.AddLe(hh14[u], model.Prod(U[u], capu[u]));
            } //对应于论文中的约束（20）



            //第15个约束条件
            INumExpr[] hh15 = new INumExpr[kitchen];
            for (int a = 0; a < kitchen; a++)
            {
                hh15[a] = Qjad[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh15[a] = model.Sum(hh15[a], Qjad[j][a]);
                }
                hh15[a] = model.Sum(hh15[a], model.Prod(-1, Qjad[0][0]));
                model.AddLe(hh15[a], model.Prod(W[a], capa[a]));
            } //对应于论文中的约束（21）


            //第16个约束条件
            INumExpr[] hh16 = new INumExpr[harmful];
            for (int v = 0; v < harmful; v++)
            {
                hh16[v] = Qjve[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh16[v] = model.Sum(hh16[v], Qjve[j][v]);
                }
                hh16[v] = model.Sum(hh16[v], model.Prod(-1, Qjve[0][0]));
                model.AddLe(hh16[v], model.Prod(V[v], capv[v]));
            } //对应于论文中的约束（22）  




            //保证所有的废物都运输到转运站
            //对应于论文中的约束（23）
            //第17个约束条件  17.1
            INumExpr[] hh171 = new INumExpr[source];
            for (int i = 0; i < source; i++)
            {
                hh171[i] = Qijb[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh171[i] = model.Sum(hh171[i], Qijb[i][j]);
                }
                hh171[i] = model.Sum(hh171[i], model.Prod(-1, Qijb[0][0]));
                model.AddGe(hh171[i], Gib[i]);
            }


            //第17个约束条件  17.2
            INumExpr[] hh172 = new INumExpr[source];
            for (int i = 0; i < source; i++)
            {
                hh172[i] = Qijc[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh172[i] = model.Sum(hh172[i], Qijc[i][j]);
                }
                hh172[i] = model.Sum(hh172[i], model.Prod(-1, Qijc[0][0]));
                model.AddGe(hh172[i], Gic[i]);
            }


            //第17个约束条件  17.3
            INumExpr[] hh173 = new INumExpr[source];
            for (int i = 0; i < source; i++)
            {
                hh173[i] = Qijd[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh173[i] = model.Sum(hh173[i], Qijd[i][j]);
                }
                hh173[i] = model.Sum(hh173[i], model.Prod(-1, Qijd[0][0]));
                model.AddGe(hh173[i], Gid[i]);
            }


            //第17个约束条件  17.4
            INumExpr[] hh174 = new INumExpr[source];
            for (int i = 0; i < source; i++)
            {
                hh174[i] = Qije[0][0];
                for (int j = 0; j < TS; j++)
                {
                    hh174[i] = model.Sum(hh174[i], Qije[i][j]);
                }
                hh174[i] = model.Sum(hh174[i], model.Prod(-1, Qije[0][0]));
                model.AddGe(hh174[i], Gie[i]);
            } //对应于论文中的约束（23）




            //转运站的最大数量约束
            //第18个约束条件
            INumExpr[] hh18 = new INumExpr[1];
            hh18[0] = X[0];
            for (int j = 0; j < TS; j++)
            {
                hh18[0] = model.Sum(hh18[0], X[j]);
            }
            hh18[0] = model.Sum(hh18[0], model.Prod(-1, X[0]));
            model.AddLe(hh18[0], B);
            //对应于论文中的约束（25）






            //至少有一个设施被建立
            //第19个约束条件
            INumExpr[] hh19 = new INumExpr[1];
            hh19[0] = X[0];
            for (int j = 0; j < TS; j++)
            {
                hh19[0] = model.Sum(hh19[0], X[j]);
            }
            hh19[0] = model.Sum(hh19[0], model.Prod(-1, X[0]));
            model.AddGe(hh19[0], 1);
            //对应于论文中的约束（26）


            //第20个约束条件
            INumExpr[] hh20 = new INumExpr[1];
            hh20[0] = Y[0];
            for (int q = 0; q < recyclable; q++)
            {
                hh20[0] = model.Sum(hh20[0], Y[q]);
            }
            hh20[0] = model.Sum(hh20[0], model.Prod(-1, Y[0]));
            model.AddGe(hh20[0], 1);
            //对应于论文中的约束（27）


            //第21个约束条件
            INumExpr[] hh21 = new INumExpr[1];
            hh21[0] = U[0];
            for (int u = 0; u < dry; u++)
            {
                hh21[0] = model.Sum(hh21[0], U[u]);
            }
            hh21[0] = model.Sum(hh21[0], model.Prod(-1, U[0]));
            model.AddGe(hh21[0], 1);
            //对应于论文中的约束（28）



            //第22个约束条件
            INumExpr[] hh22 = new INumExpr[1];
            hh22[0] = W[0];
            for (int w = 0; w < kitchen; w++)
            {
                hh22[0] = model.Sum(hh22[0], W[w]);
            }
            hh22[0] = model.Sum(hh22[0], model.Prod(-1, W[0]));
            model.AddGe(hh22[0], 1);
            //对应于论文中的约束（29）



            //第23个约束条件
            INumExpr[] hh23 = new INumExpr[1];
            hh23[0] = V[0];
            for (int v = 0; v < harmful; v++)
            {
                hh23[0] = model.Sum(hh23[0], V[v]);
            }
            hh23[0] = model.Sum(hh23[0], model.Prod(-1, V[0]));
            model.AddGe(hh23[0], 1);
            //对应于论文中的约束（30）











            //目标函数1
            INumExpr[] obj11 = new INumExpr[5];
            INumExpr[] obj12 = new INumExpr[8];
            INumExpr[] obj13 = new INumExpr[8];
            INumExpr[] obj14 = new INumExpr[3];
            obj11[0] = X[0];
            obj11[1] = Y[0];
            obj11[2] = U[0];
            obj11[3] = W[0];
            obj11[4] = V[0];

            for (int j = 0; j < TS; j++)
            {
                obj11[0] = model.Sum(obj11[0], model.Prod(X[j], FCj[j]));
            }
            obj11[0] = model.Sum(obj11[0], model.Prod(-1, X[0]));//转运站的固定成本

            for (int q = 0; q < recyclable; q++)
            {
                obj11[1] = model.Sum(obj11[1], model.Prod(Y[q], FCq[q]));
            }
            obj11[1] = model.Sum(obj11[1], model.Prod(-1, Y[0])); //可回收垃圾处理厂的固定成本

            for (int u = 0; u < dry; u++)
            {
                obj11[2] = model.Sum(obj11[2], model.Prod(U[u], FCu[u]));
            }
            obj11[2] = model.Sum(obj11[2], model.Prod(-1, U[0])); //干垃圾处理厂的固定成本

            for (int a = 0; a < kitchen; a++)
            {
                obj11[3] = model.Sum(obj11[3], model.Prod(W[a], FCa[a]));
            }
            obj11[3] = model.Sum(obj11[3], model.Prod(-1, W[0])); //湿垃圾处理厂的固定成本

            for (int v = 0; v < harmful; v++)
            {
                obj11[4] = model.Sum(obj11[4], model.Prod(V[v], FCv[v]));
            }
            obj11[4] = model.Sum(obj11[4], model.Prod(-1, V[0])); //湿垃圾处理厂的固定成本
            obj11[0] = model.Sum(model.Sum(obj11[0], obj11[1]), model.Sum(obj11[2], obj11[3]));
            obj11[0] = model.Sum(obj11[0], obj11[4]);                                           //固定成本之和


            obj12[0] = Qijb[0][0];
            obj12[1] = Qijc[0][0];
            obj12[2] = Qijd[0][0];
            obj12[3] = Qije[0][0];

            obj12[4] = Qijb[0][0];
            obj12[5] = Qijc[0][0];
            obj12[6] = Qijd[0][0];
            obj12[7] = Qije[0][0];

            for (int i = 0; i < source; i++)
            {
                for (int j = 0; j < TS; j++)
                {
                    obj12[0] = model.Sum(obj12[0], model.Prod(Qijb[i][j], dij[i][j]));
                    obj12[1] = model.Sum(obj12[1], model.Prod(Qijc[i][j], dij[i][j]));
                    obj12[2] = model.Sum(obj12[2], model.Prod(Qijd[i][j], dij[i][j]));
                    obj12[3] = model.Sum(obj12[3], model.Prod(Qije[i][j], dij[i][j]));
                }
            }
            obj12[0] = model.Sum(obj12[0], model.Prod(Qijb[0][0], -1));
            obj12[1] = model.Sum(obj12[1], model.Prod(Qijc[0][0], -1));
            obj12[2] = model.Sum(obj12[2], model.Prod(Qijd[0][0], -1));
            obj12[3] = model.Sum(obj12[3], model.Prod(Qije[0][0], -1));
            obj12[0] = model.Sum(model.Sum(obj12[0], obj12[1]), model.Sum(obj12[2], obj12[3]));
            obj12[0] = model.Prod(obj12[0], TCij);//i-j的运输成本

            for (int i = 0; i < source; i++)
            {
                for (int j = 0; j < TS; j++)
                {
                    obj12[4] = model.Sum(obj12[4], model.Prod(Qijb[i][j], OCj[j]));
                    obj12[5] = model.Sum(obj12[5], model.Prod(Qijc[i][j], OCj[j]));
                    obj12[6] = model.Sum(obj12[6], model.Prod(Qijd[i][j], OCj[j]));
                    obj12[7] = model.Sum(obj12[7], model.Prod(Qije[i][j], OCj[j]));
                }
            }
            obj12[4] = model.Sum(obj12[4], model.Prod(Qijb[0][0], -1));
            obj12[5] = model.Sum(obj12[5], model.Prod(Qijc[0][0], -1));
            obj12[6] = model.Sum(obj12[6], model.Prod(Qijd[0][0], -1));
            obj12[7] = model.Sum(obj12[7], model.Prod(Qije[0][0], -1));
            obj12[4] = model.Sum(model.Sum(obj12[4], obj12[5]), model.Sum(obj12[6], obj12[7]));//j的运营成本


            obj13[0] = Qjqb[0][0];
            obj13[1] = Qjuc[0][0];
            obj13[2] = Qjad[0][0];
            obj13[3] = Qjve[0][0];

            obj13[4] = Qjqb[0][0];
            obj13[5] = Qjuc[0][0];
            obj13[6] = Qjad[0][0];
            obj13[7] = Qjve[0][0];

            for (int j = 0; j < TS; j++)
            {
                for (int q = 0; q < recyclable; q++)
                {
                    obj13[0] = model.Sum(obj13[0], model.Prod(Qjqb[j][q], djq[j][q]));
                    obj13[4] = model.Sum(obj13[4], model.Prod(Qjqb[j][q], OCq[q]));
                }
            }
            obj13[0] = model.Sum(obj13[0], model.Prod(Qjqb[0][0], -1));
            obj13[4] = model.Sum(obj13[4], model.Prod(Qjqb[0][0], -1));

            for (int j = 0; j < TS; j++)
            {
                for (int u = 0; u < dry; u++)
                {
                    obj13[1] = model.Sum(obj13[1], model.Prod(Qjuc[j][u], dju[j][u]));
                    obj13[5] = model.Sum(obj13[5], model.Prod(Qjuc[j][u], OCu[u]));
                }
            }
            obj13[1] = model.Sum(obj13[1], model.Prod(Qjuc[0][0], -1));
            obj13[5] = model.Sum(obj13[5], model.Prod(Qjuc[0][0], -1));

            for (int j = 0; j < TS; j++)
            {
                for (int a = 0; a < kitchen; a++)
                {
                    obj13[2] = model.Sum(obj13[2], model.Prod(Qjad[j][a], dja[j][a]));
                    obj13[6] = model.Sum(obj13[6], model.Prod(Qjad[j][a], OCa[a]));
                }
            }
            obj13[2] = model.Sum(obj13[2], model.Prod(Qjad[0][0], -1));
            obj13[6] = model.Sum(obj13[6], model.Prod(Qjad[0][0], -1));

            for (int j = 0; j < TS; j++)
            {
                for (int v = 0; v < harmful; v++)
                {
                    obj13[3] = model.Sum(obj13[3], model.Prod(Qjve[j][v], djv[j][v]));
                    obj13[7] = model.Sum(obj13[7], model.Prod(Qjve[j][v], OCv[v]));
                }
            }
            obj13[3] = model.Sum(obj13[3], model.Prod(Qjve[0][0], -1));
            obj13[7] = model.Sum(obj13[7], model.Prod(Qjve[0][0], -1));

            obj13[0] = model.Sum(model.Sum(obj13[0], obj13[1]), model.Sum(obj13[2], obj13[3]));
            obj13[0] = model.Prod(obj13[0], TCjk);//j-k的运输成本
            obj13[4] = model.Sum(model.Sum(obj13[4], obj13[5]), model.Sum(obj13[6], obj13[7]));//k的运营成本
            obj13[0] = model.Sum(model.Sum(obj12[0], obj12[4]), model.Sum(obj13[0], obj13[4]));               //运输成本+运营成本之和

            obj14[0] = Qjqb[0][0];
            obj14[1] = Qjuc[0][0];
            obj14[2] = Qjad[0][0];
            for (int j = 0; j < TS; j++)
            {
                for (int q = 0; q < recyclable; q++)
                {
                    obj14[0] = model.Sum(obj14[0], model.Prod(Qjqb[j][q], rateq));
                }
            }
            obj14[0] = model.Sum(obj14[0], model.Prod(Qjqb[0][0], -1));
            obj14[0] = model.Prod(obj14[0], pr2);//可回收垃圾转化为产品时的收益

            for (int j = 0; j < TS; j++)
            {
                for (int u = 0; u < dry; u++)
                {
                    obj14[1] = model.Sum(obj14[1], model.Prod(Qjuc[j][u], rateu));
                }
            }
            obj14[1] = model.Sum(obj14[1], model.Prod(Qjuc[0][0], -1));
            obj14[1] = model.Prod(obj14[1], pr1);//干垃圾转化为产品时的收益

            for (int j = 0; j < TS; j++)
            {
                for (int a = 0; a < kitchen; a++)
                {
                    obj14[2] = model.Sum(obj14[2], model.Prod(Qjad[j][a], ratea));
                }
            }
            obj14[2] = model.Sum(obj14[2], model.Prod(Qjad[0][0], -1));
            obj14[2] = model.Prod(obj14[2], pr1);//湿垃圾转化为产品时的收益
            obj14[0] = model.Sum(obj14[0], model.Sum(obj14[1], obj14[2]));               //产品收益之和

            obj11[0] = model.Sum(model.Sum(obj11[0], obj13[0]), model.Prod(obj14[0], -1));

            model.AddMinimize(obj11[0]);  //总成本最小化






            //目标函数2
            INumExpr[] obj21 = new INumExpr[8];
            INumExpr[] obj22 = new INumExpr[8];
            obj21[0] = Qijb[0][0];
            obj21[1] = Qijc[0][0];
            obj21[2] = Qijd[0][0];
            obj21[3] = Qije[0][0];

            obj21[4] = Qijb[0][0];
            obj21[5] = Qijc[0][0];
            obj21[6] = Qijd[0][0];
            obj21[7] = Qije[0][0];

            for (int i = 0; i < source; i++)
            {
                for (int j = 0; j < TS; j++)
                {
                    obj21[0] = model.Sum(obj21[0], model.Prod(Qijb[i][j], dij[i][j]));
                    obj21[1] = model.Sum(obj21[1], model.Prod(Qijc[i][j], dij[i][j]));
                    obj21[2] = model.Sum(obj21[2], model.Prod(Qijd[i][j], dij[i][j]));
                    obj21[3] = model.Sum(obj21[3], model.Prod(Qije[i][j], dij[i][j]));
                }
            }
            obj21[0] = model.Sum(obj21[0], model.Prod(Qijb[0][0], -1));
            obj21[1] = model.Sum(obj21[1], model.Prod(Qijc[0][0], -1));
            obj21[2] = model.Sum(obj21[2], model.Prod(Qijd[0][0], -1));
            obj21[3] = model.Sum(obj21[3], model.Prod(Qije[0][0], -1));
            obj21[0] = model.Sum(model.Sum(obj21[0], obj21[1]), model.Sum(obj21[2], obj21[3]));
            obj21[0] = model.Prod(obj21[0], fuelij);//收集车辆运输过程中产生的碳排放

            for (int i = 0; i < source; i++)
            {
                for (int j = 0; j < TS; j++)
                {
                    obj21[4] = model.Sum(obj21[4], model.Prod(Qijb[i][j], fjb));
                    obj21[5] = model.Sum(obj21[5], model.Prod(Qijc[i][j], fjc));
                    obj21[6] = model.Sum(obj21[6], model.Prod(Qijd[i][j], fjd));
                    obj21[7] = model.Sum(obj21[7], model.Prod(Qije[i][j], fje));
                }
            }
            obj21[4] = model.Sum(obj21[4], model.Prod(Qijb[0][0], -1));
            obj21[5] = model.Sum(obj21[5], model.Prod(Qijc[0][0], -1));
            obj21[6] = model.Sum(obj21[6], model.Prod(Qijd[0][0], -1));
            obj21[7] = model.Sum(obj21[7], model.Prod(Qije[0][0], -1));
            obj21[4] = model.Sum(model.Sum(obj21[4], obj21[5]), model.Sum(obj21[6], obj21[7]));//转运站处理废物过程中产生的碳排放



            obj22[0] = Qjqb[0][0];
            obj22[1] = Qjuc[0][0];
            obj22[2] = Qjad[0][0];
            obj22[3] = Qjve[0][0];

            obj22[4] = Qjqb[0][0];
            obj22[5] = Qjuc[0][0];
            obj22[6] = Qjad[0][0];
            obj22[7] = Qjve[0][0];

            for (int j = 0; j < TS; j++)
            {
                for (int q = 0; q < recyclable; q++)
                {
                    obj22[0] = model.Sum(obj22[0], model.Prod(Qjqb[j][q], djq[j][q]));
                    obj22[4] = model.Sum(obj22[4], model.Prod(Qjqb[j][q], fq));
                }
            }
            obj22[0] = model.Sum(obj22[0], model.Prod(Qjqb[0][0], -1));
            obj22[4] = model.Sum(obj22[4], model.Prod(Qjqb[0][0], -1));

            for (int j = 0; j < TS; j++)
            {
                for (int u = 0; u < dry; u++)
                {
                    obj22[1] = model.Sum(obj22[1], model.Prod(Qjuc[j][u], dju[j][u]));
                    obj22[5] = model.Sum(obj22[5], model.Prod(Qjuc[j][u], fu));
                }
            }
            obj22[1] = model.Sum(obj22[1], model.Prod(Qjuc[0][0], -1));
            obj22[5] = model.Sum(obj22[5], model.Prod(Qjuc[0][0], -1));

            for (int j = 0; j < TS; j++)
            {
                for (int a = 0; a < kitchen; a++)
                {
                    obj22[2] = model.Sum(obj22[2], model.Prod(Qjad[j][a], dja[j][a]));
                    obj22[6] = model.Sum(obj22[6], model.Prod(Qjad[j][a], fa));
                }
            }
            obj22[2] = model.Sum(obj22[2], model.Prod(Qjad[0][0], -1));
            obj22[6] = model.Sum(obj22[6], model.Prod(Qjad[0][0], -1));

            for (int j = 0; j < TS; j++)
            {
                for (int v = 0; v < harmful; v++)
                {
                    obj22[3] = model.Sum(obj22[3], model.Prod(Qjve[j][v], djv[j][v]));
                    obj22[7] = model.Sum(obj22[7], model.Prod(Qjve[j][v], fv));
                }
            }
            obj22[3] = model.Sum(obj22[3], model.Prod(Qjve[0][0], -1));
            obj22[7] = model.Sum(obj22[7], model.Prod(Qjve[0][0], -1));

            obj22[0] = model.Sum(model.Sum(obj22[0], obj22[1]), model.Sum(obj22[2], obj22[3]));
            obj22[0] = model.Prod(obj22[0], fueljk);//运输车辆运输过程中产生的碳排放
            obj22[4] = model.Sum(model.Sum(obj22[4], obj22[5]), model.Sum(obj22[6], obj22[7]));//处理设施处理废物过程中产生的碳排放
            obj21[0] = model.Sum(model.Sum(obj21[0], obj21[4]), model.Sum(obj22[0], obj22[4]));
            obj21[0] = model.Prod(obj21[0], Zo);

            obj21[0] = model.Prod(obj21[0], bo);//碳排放成本总和

            //model.AddMinimize(obj21[0]);碳排放成本总和最小化
            model.AddLe(obj21[0], M);//ε碳排放约束








            //目标函数3
            INumExpr[] obj3 = new INumExpr[8];
            INumExpr[] obj31 = new INumExpr[4];
            obj3[0] = X[0];
            obj3[1] = Y[0];
            obj3[2] = U[0];
            obj3[3] = W[0];
            obj3[4] = V[0];

            for (int j = 0; j < TS; j++)
            {
                obj31[0] = Qijb[0][0];
                obj31[1] = Qijc[0][0];
                obj31[2] = Qijd[0][0];
                obj31[3] = Qije[0][0];
                for (int i = 0; i < source; i++)
                {
                    obj31[0] = model.Sum(obj31[0], model.Prod(Qijb[i][j], mjobj[j]));
                    obj31[1] = model.Sum(obj31[1], model.Prod(Qijc[i][j], mjobj[j]));
                    obj31[2] = model.Sum(obj31[2], model.Prod(Qijd[i][j], mjobj[j]));
                    obj31[3] = model.Sum(obj31[3], model.Prod(Qije[i][j], mjobj[j]));
                }
                obj31[0] = model.Sum(obj31[0], model.Prod(Qijb[0][0], -1));
                obj31[1] = model.Sum(obj31[1], model.Prod(Qijc[0][0], -1));
                obj31[2] = model.Sum(obj31[2], model.Prod(Qijd[0][0], -1));
                obj31[3] = model.Sum(obj31[3], model.Prod(Qije[0][0], -1));

                obj31[0] = model.Sum(model.Sum(obj31[0], obj31[1]), model.Sum(obj31[2], obj31[3]));
                obj31[0] = model.Sum(obj31[0], fjobj[j]);
                obj31[0] = model.Prod(model.Prod(obj31[0], X[j]), regionj[j]);
                obj3[0] = model.Sum(obj3[0], obj31[0]);
            }
            obj3[0] = model.Sum(obj3[0], model.Prod(X[0], -1));//转运站j的社会效益

            INumExpr[] obj32 = new INumExpr[1];
            for (int q = 0; q < recyclable; q++)
            {
                obj32[0] = Qjqb[0][0];
                for (int j = 0; j < TS; j++)
                {
                    obj32[0] = model.Sum(obj32[0], model.Prod(Qjqb[j][q], mjobq[q]));
                }
                obj32[0] = model.Sum(obj32[0], model.Prod(Qjqb[0][0], -1));
                obj32[0] = model.Sum(obj32[0], fjobq[q]);
                obj32[0] = model.Prod(model.Prod(obj32[0], Y[q]), regionq[q]);
                obj3[1] = model.Sum(obj3[1], obj32[0]);
            }
            obj3[1] = model.Sum(obj3[1], model.Prod(Y[0], -1));//可回收垃圾处理厂q的社会效益

            INumExpr[] obj33 = new INumExpr[1];
            for (int u = 0; u < dry; u++)
            {
                obj33[0] = Qjuc[0][0];
                for (int j = 0; j < TS; j++)
                {
                    obj33[0] = model.Sum(obj33[0], model.Prod(Qjuc[j][u], mjobu[u]));
                }
                obj33[0] = model.Sum(obj33[0], model.Prod(Qjuc[0][0], -1));
                obj33[0] = model.Sum(obj33[0], fjobu[u]);
                obj33[0] = model.Prod(model.Prod(obj33[0], U[u]), regionu[u]);
                obj3[2] = model.Sum(obj3[2], obj33[0]);
            }
            obj3[2] = model.Sum(obj3[2], model.Prod(U[0], -1));//干垃圾处理厂u的社会效益

            INumExpr[] obj34 = new INumExpr[1];
            for (int a = 0; a < kitchen; a++)
            {
                obj34[0] = Qjad[0][0];
                for (int j = 0; j < TS; j++)
                {
                    obj34[0] = model.Sum(obj34[0], model.Prod(Qjad[j][a], mjoba[a]));
                }
                obj34[0] = model.Sum(obj34[0], model.Prod(Qjad[0][0], -1));
                obj34[0] = model.Sum(obj34[0], fjoba[a]);
                obj34[0] = model.Prod(model.Prod(obj34[0], W[a]), regiona[a]);
                obj3[3] = model.Sum(obj3[3], obj34[0]);
            }
            obj3[3] = model.Sum(obj3[3], model.Prod(W[0], -1));//湿垃圾处理厂a的社会效益

            INumExpr[] obj35 = new INumExpr[1];
            for (int v = 0; v < harmful; v++)
            {
                obj35[0] = Qjve[0][0];
                for (int j = 0; j < TS; j++)
                {
                    obj35[0] = model.Sum(obj35[0], model.Prod(Qjve[j][v], mjobv[v]));
                }
                obj35[0] = model.Sum(obj35[0], model.Prod(Qjve[0][0], -1));
                obj35[0] = model.Sum(obj35[0], fjobv[v]);
                obj35[0] = model.Prod(model.Prod(obj35[0], V[v]), regionv[v]);
                obj3[4] = model.Sum(obj3[4], obj35[0]);
            }
            obj3[4] = model.Sum(obj3[4], model.Prod(V[0], -1));//有害垃圾处理厂v的社会效益

            obj3[0] = model.Sum(model.Sum(obj3[0], obj3[1]), model.Sum(obj3[2], obj3[3]));
            obj3[0] = model.Sum(obj3[0], obj3[4]);//社会效益之和

            // model.AddMaximize(obj3[0]);
            model.AddGe(obj3[0], 2);//ε社会效益约束










            //输出决策变量

            if (model.Solve())
            {

                System.Console.WriteLine("obj=" + model.ObjValue);

                for (int i = 0; i < source; i++)
                {
                    for (int j = 0; j < TS; j++)
                    {
                        if (model.GetValues(Qijb[i])[j] != 0)
                        {
                            System.Console.WriteLine("Qijb[" + i.ToString() + "][" + j.ToString() + "]=" + model.GetValues(Qijb[i])[j].ToString());
                        }
                    }
                }//输出决策变量i-j运输可回收垃圾b的运量Qijb[i][j]

                for (int i = 0; i < source; i++)
                {
                    for (int j = 0; j < TS; j++)
                    {
                        if (model.GetValues(Qijc[i])[j] != 0)
                        {
                            System.Console.WriteLine("Qijc[" + i.ToString() + "][" + j.ToString() + "]=" + model.GetValues(Qijc[i])[j].ToString());
                        }
                    }
                }//输出决策变量i-j运输干垃圾c的运量Qijc[i][j]


                for (int i = 0; i < source; i++)
                {
                    for (int j = 0; j < TS; j++)
                    {
                        if (model.GetValues(Qijd[i])[j] != 0)
                        {
                            System.Console.WriteLine("Qijd[" + i.ToString() + "][" + j.ToString() + "]=" + model.GetValues(Qijd[i])[j].ToString());
                        }
                    }
                }//输出决策变量i-j运输湿垃圾d的运量Qijd[i][j]


                for (int i = 0; i < source; i++)
                {
                    for (int j = 0; j < TS; j++)
                    {
                        if (model.GetValues(Qije[i])[j] != 0)
                        {
                            System.Console.WriteLine("Qije[" + i.ToString() + "][" + j.ToString() + "]=" + model.GetValues(Qije[i])[j].ToString());
                        }
                    }
                }//输出决策变量i-j运输有害垃圾e的运量Qije[i][j]




                for (int j = 0; j < TS; j++)
                {
                    for (int q = 0; q < recyclable; q++)
                    {
                        if (model.GetValues(Qjqb[j])[q] != 0)
                        {
                            System.Console.WriteLine("Qjqb[" + j.ToString() + "][" + q.ToString() + "]=" + model.GetValues(Qjqb[j])[q].ToString());
                        }
                    }
                }//输出决策变量j-q运输可回收垃圾b的运量Qjqb[j][q]


                for (int j = 0; j < TS; j++)
                {
                    for (int u = 0; u < dry; u++)
                    {
                        if (model.GetValues(Qjuc[j])[u] != 0)
                        {
                            System.Console.WriteLine("Qjuc[" + j.ToString() + "][" + u.ToString() + "]=" + model.GetValues(Qjuc[j])[u].ToString());
                        }
                    }
                }//输出决策变量j-u运输干垃圾c的运量Qjuc[j][u]


                for (int j = 0; j < TS; j++)
                {
                    for (int a = 0; a < kitchen; a++)
                    {
                        if (model.GetValues(Qjad[j])[a] != 0)
                        {
                            System.Console.WriteLine("Qjad[" + j.ToString() + "][" + a.ToString() + "]=" + model.GetValues(Qjad[j])[a].ToString());
                        }
                    }
                }//输出决策变量j-a运输湿垃圾d的运量Qjad[j][a]


                for (int j = 0; j < TS; j++)
                {
                    for (int v = 0; v < harmful; v++)
                    {
                        if (model.GetValues(Qjve[j])[v] != 0)
                        {
                            System.Console.WriteLine("Qjve[" + j.ToString() + "][" + v.ToString() + "]=" + model.GetValues(Qjve[j])[v].ToString());
                        }
                    }
                }//输出决策变量j-e运输有害垃圾e的运量Qjve[j][v]



                for (int j = 0; j < TS; j++)
                {
                    if (model.GetValues(X)[j] != 0)
                    {
                        System.Console.WriteLine("X[" + j.ToString() + "]=" + model.GetValues(X)[j].ToString());
                    }
                }//输出决策变量转运站j是否开放（0,1）X[j]


                for (int q = 0; q < recyclable; q++)
                {
                    if (model.GetValues(Y)[q] != 0)
                    {
                        System.Console.WriteLine("Y[" + q.ToString() + "]=" + model.GetValues(Y)[q].ToString());
                    }
                }//输出决策变量可回收垃圾处理厂q是否开放（0,1）Y[q]


                for (int u = 0; u < dry; u++)
                {
                    if (model.GetValues(U)[u] != 0)
                    {
                        System.Console.WriteLine("U[" + u.ToString() + "]=" + model.GetValues(U)[u].ToString());
                    }
                }//输出决策变量干垃圾处理厂是否开放（0,1）U[u]


                for (int a = 0; a < kitchen; a++)
                {
                    if (model.GetValues(W)[a] != 0)
                    {
                        System.Console.WriteLine("W[" + a.ToString() + "]=" + model.GetValues(W)[a].ToString());
                    }
                }//输出决策变量湿垃圾处理厂是否开放（0,1）W[a]

                for (int v = 0; v < harmful; v++)
                {
                    if (model.GetValues(V)[v] != 0)
                    {
                        System.Console.WriteLine("V[" + v.ToString() + "]=" + model.GetValues(V)[v].ToString());
                    }
                }//输出决策变量有害垃圾处理厂是否开放（0,1）V[v]




                model.ExportModel("Transport casualties.LP");//得到.LP文件
                DateTime T = DateTime.Now;
                System.TimeSpan C = T - A;//得到运行时间
                System.Console.WriteLine("程序运行时间为：{0}", C);//输出运行时间
                Console.Read();

                // System.Console.ReadKey();

            }


        }
    }
}

