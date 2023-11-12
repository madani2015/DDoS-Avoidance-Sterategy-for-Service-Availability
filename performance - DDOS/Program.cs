using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using NPOI.HSSF.UserModel;
using System.Windows.Forms;

namespace performance___DDOS
{
    class packet
    {
        public int time;
        public int load;
    }
    class Program
    {
        public static result_generator res_gen;
        public static List<packet> temp_queue;
        
        static void Main(string[] args)
        {
            temp_queue = new List<packet>();
            sim_init();
            while (Console.ReadKey().Key == ConsoleKey.Y)
            {
                temp_queue.Clear();
                sim_init();
                temp_queue.Clear();
            }

        }
        
        static void sim_init()
        {
            Console.WriteLine("\nInitiating the system.");
            res_gen = new result_generator();

            new sim(sim.sys_state.normal);
            new sim(sim.sys_state.ddos);
            res_gen.save_report();
            new Form1().ShowDialog();
            Console.Write("Run again (Y/N):");
        }
    }

    class sim
    {
        public enum sys_state
        {
            normal,
            ddos
        }

        public static int overal_time;        
        public int sim_clock;
        int total_load;
        public static int T1, T2;
        public static int ddos_time;
        sys_state sys_mode;
        Queue<packet> server_queue;
        Queue<packet> avoidance_queue;
        List<packet> pkt_raw_result;
        List<packet> pkt_analyzed_result;
        //System.IO.TextWriter wr = new System.IO.StreamWriter("D:\\ddos-log.txt");
        //Troschuetz.Random.XorShift128Generator rnd = new Troschuetz.Random.XorShift128Generator();
        Troschuetz.Random.StandardGenerator rnd = new Troschuetz.Random.StandardGenerator();
        public sim(sys_state sys)
        {
            overal_time = 1001;
            ddos_time = 101;
            sys_mode = sys_state.normal;
            pkt_raw_result = new List<packet>();
            pkt_analyzed_result = new List<packet>();
            server_queue = new Queue<packet>();
            avoidance_queue = new Queue<packet>();
            T1 = 2000;
            T2 = 3000;
            total_load = 0;
            sim_clock = 0;
            client(sys);
        }
        void server()
        {
            var pk1 = server_queue.Dequeue();
            var pk2 = server_queue.Dequeue();
            //Console.Write(" {0}:{1}:{2}:{3} ", new object[] { pk1.time, pk1.load,pk2.load,pk1.load+pk2.load });
            pkt_raw_result.Add(new packet() { load = pk1.load + pk2.load, time = pk1.time });
            sim_clock++;
            avoidance_queue.Enqueue(pk1);
            avoidance_queue.Enqueue(pk2);
        }
        void client(sys_state st)
        {
            Console.WriteLine("\nSystem Running...");
            if (st == sys_state.normal) Console.WriteLine("\nNormal packet generating...");
            else Console.WriteLine("\nDDOS Storm...");
            //TimerCallback normal = new TimerCallback(normal_Tick);
            //TimerCallback ddos = new TimerCallback(ddos_Tick);
            //stateTimer = new Timer(normal, null, 0, 1);
            while (sim_clock < ddos_time && st != sys_state.ddos)
            {
                normal_Tick();
            }
            /*
            if (sys_mode == sys_state.ddos)
            {
                stateTimer.Dispose();
                Console.WriteLine("\nDDOS attack...");
                stateTimer = new Timer(ddos, null, 0, 1);
            }
             */
            if (st == sys_state.ddos)
            {
                pkt_raw_result.AddRange(Program.temp_queue);
                foreach (var it in Program.temp_queue)
                {
                    avoidance_queue.Enqueue(it);
                }
                sim_clock = 102;
                sys_mode = sys_state.ddos;
            }
            while (sim_clock < overal_time)
            {
                normal_Tick();
            }
            while (pkt_raw_result.Count < overal_time)
            {
            }
            //stateTimer.Dispose();
            Console.WriteLine("\nEnd of generating.");
            //Console.Write("\nAnalyze the packets (Y/N)?");
            //if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("");
                /*
                List<string> tmp = new List<string>();
                for (int i = 0; i < pkt_raw_result.Count; i++)
                {

                    tmp.Add(pkt_raw_result[i].time + ":" + pkt_raw_result[i].load);
                    //wr.WriteLine(" {0}:{1} ", new object[] { pkt_raw_result[i].time, pkt_raw_result[i].load });
                    Console.Write(" {0}:{1} ", new object[] { pkt_raw_result[i].time, pkt_raw_result[i].load });
                }
                File.WriteAllLines("d:\\ddos-log2.txt", tmp);
                 */
                if (sys_mode == sys_state.ddos)
                    Program.res_gen.generate(pkt_raw_result, "DDOS");
                else
                {
                    Program.res_gen.generate(pkt_raw_result, "NORMAL");
                    Program.temp_queue.AddRange(pkt_raw_result.Where(c => c.time < ddos_time+1).AsEnumerable());

                }

            }
            if(sys_mode==sys_state.ddos)
            {
                //Console.Write("\nRunning avoidance strategy (Y/N)?");
                //if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    avoidance();
                }
            }
        }
        packet packet_generation(int min_time,int max_time)
        {
            packet pkt = new packet();
            pkt.time = sim_clock;
            pkt.load = rnd.Next(min_time, max_time);
            return pkt;
        }

        public void normal_Tick()
        {
            var load=packet_generation(0, 500);
            server_queue.Enqueue(load);
            if (sys_mode == sys_state.ddos)
            {
                //Console.WriteLine("\nDDOS attack...");
                ddos_Tick(2000, 2700);
                //stateTimer = new Timer(ddos, null, 0, 1);
            }
            else
            {
                ddos_Tick(0, 500);
            }

        }
        public void ddos_Tick(int min,int max)
        {
            var load = packet_generation(min, max);
            server_queue.Enqueue(load);
            server();
        }

        void avoidance()
        {
            bool ddos_flag = false;
             //if current traffic > T1 : Red state
            Console.WriteLine("Analyzing the traffic...");
            while (avoidance_queue.Count>0)
            {
                var pk1 = avoidance_queue.Dequeue();
                var pk2 = avoidance_queue.Dequeue();
                if (pk2.time < ddos_time+1)
                {
                    pkt_analyzed_result.Add(new packet() { time = pk1.time, load = pk1.load});
                    pkt_analyzed_result.Add(new packet() { time = pk2.time, load = pk2.load});
                }
                else
                {
                    if (pk1.load + pk2.load >= T2)
                    {
                        ddos_flag = true;
                    }
                    if (pk1.load + pk2.load >= T1 && ddos_flag)
                    {
                        if (pk1.load < 1000) pkt_analyzed_result.Add(pk1);
                        else pkt_analyzed_result.Add(pk2);
                    }
                    else
                    {
                        pkt_analyzed_result.Add(new packet() { time = pk1.time, load = pk1.load + pk2.load });
                    }
                }
            }
            Console.Write("Analyzing finished.");
            //if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("");
                /*
                List<string> tmp = new List<string>();
                for (int i = 0; i < pkt_analyzed_result.Count; i++)
                {

                    tmp.Add(pkt_analyzed_result[i].time + ":" + pkt_analyzed_result[i].load);
                    //wr.WriteLine(" {0}:{1} ", new object[] { pkt_raw_result[i].time, pkt_raw_result[i].load });
                    Console.Write(" {0}:{1} ", new object[] { pkt_analyzed_result[i].time, pkt_analyzed_result[i].load });
                }
                File.WriteAllLines("d:\\ddos-avoidance-log.txt", tmp);
                 */
                Program.res_gen.generate(pkt_analyzed_result, "avoidance");
            }

        }
    }

    class result_generator
    {
        HSSFWorkbook workbook;
        public result_generator()
        {
            workbook = new HSSFWorkbook();
        }

        public void generate(List<packet> pkt,string sheet_name)
        {

            // Create a new workbook and a sheet named "Floats"
            
            var sheet = workbook.CreateSheet(sheet_name);

            var rowIndex = 0;
            foreach (var n in pkt)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(n.time);
                row.CreateCell(1).SetCellValue(n.load);
                rowIndex++;
            }
        }

        public void save_report()
        {
            using (var fileData = new FileStream(Application.StartupPath+"\\ddos_result.xls", FileMode.Create))
            {
                workbook.Write(fileData);
            }
        }
    }
}

