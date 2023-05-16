using System.IO;
using System.Text.RegularExpressions;
using System;

namespace ResRegV1cons
{
    class ResAreBusy : Exception { }
    class ResIdInvalid : Exception { }
    class UnRecommended : Exception { }
    class ResIsBusy : Exception { }
    class ResWasFree : Exception { }
    class InvalidCommandFormat : Exception { }
    static class SetUp
    {
        public static string Path; //путь к файлу, сохраняющему модель
        private static void ClearModel()
        {
            Console.WriteLine("Укажите количество ресурсов:");
            try
            {
                Model.vRes_s = new string[Convert.ToInt32(Console.ReadLine())];
                for (int i = 0; i < Model.vRes_s.Length; i++) Model.vRes_s[i] = "F";
            }
            catch
            {
                Console.WriteLine("Введено некорректное число!");
                ClearModel();
            }
        }
        private static void GetModel()
        {
            Console.WriteLine("Обновить файл?");
            if (Console.ReadLine().ToUpper() == "Y") ClearModel();
            else
            {
                Model.vRes_s = File.ReadAllLines(Path);
            }
        }
        public static bool On()
        {
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\Resmod00"))
                {
                    Console.WriteLine("Использовать существующий стандартный файл Resmod00?");
                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        Path = Directory.GetCurrentDirectory() + @"\Resmod00";
                        GetModel();
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("Создать стандартный файл?");
                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        Path = Directory.GetCurrentDirectory() + @"\Resmod00";
                        ClearModel();
                        return true;
                    }
                };
                Console.WriteLine("Введите полный адрес нестандартного файла:");
                Path = Console.ReadLine();
                if (File.Exists(Path))
                {
                    GetModel();
                    return true;
                }
                else
                {
                    ClearModel();
                    return true;
                }
            }
            catch (IOException) { Console.WriteLine("Файл не открылся."); return false; }
            catch (Exception) { Console.WriteLine("Ошибка ввода-вывода."); return false; }
        }

        
    }

    

    static class Model
    {
        public static string[] vRes_s;//Модель набора ресурсов
        public static string[] vRes_s_buf;//буфер модели набора ресурсов
        public static void Occupy(string cn)
        {
            if ((Convert.ToInt16(cn) > vRes_s.Length) | (Convert.ToInt16(cn) < 0)) throw new ResIdInvalid();
            if (vRes_s[Convert.ToInt16(cn) - 1] == "B") throw new ResIsBusy();
            vRes_s[Convert.ToInt16(cn) - 1] = "B";
        } 
        public static void Free(string cn)
        {
            if ((Convert.ToInt16(cn) > vRes_s.Length) | (Convert.ToInt16(cn) < 0)) throw new ResIdInvalid();
            if (vRes_s[Convert.ToInt16(cn) - 1] == "F") throw new ResWasFree();
            vRes_s[Convert.ToInt16(cn) - 1] = "F";
        }
        public static string Request()
        {
            for (int i = 0; i < vRes_s.Length; i++)
            {
                if (vRes_s[i] == "F") return Convert.ToString(i + 1);
            }
            throw new ResAreBusy(); ;
        }

        public static bool Target_Request(int target)
        {
            if (vRes_s[target] == "F") return true;
            else return false;
        }

        public static bool Multi_Request(int[] res_arr) 
        {
            foreach (int res in res_arr) 
            {
                if (vRes_s[res] == "B") 
                {
                    return false;
                }
            }
            return true;
        }
    }


    











class Program
{



    public class Single_command
    {
        public string bdy;
        public string prm;
        public Single_command()
        {
            bdy = "default";
            prm = "tluafed";
        }
    }

    public class Commander : Single_command
    {

        private string[] Body;
        private string[] Param;
        public int Cmd_cnt;



        public Single_command Single_command_returner(int ndx)
        {

            Single_command temp = new Single_command();
            temp.bdy = Body[ndx];
            temp.prm = Param[ndx];

            return (temp);

        }

        public Commander() 
        {

        }

        public Commander(string loaden)
        {
                Cmd_cnt = 0;
                string[] Temp_Body = loaden.Split('>');
                string[] _Body = new string[Temp_Body.Length];
                string[] _Param = new string[Temp_Body.Length];

                foreach (var sub in Temp_Body)
                {
                    string subsub = sub.ToUpper();
                    subsub = subsub.Trim();
                    Regex Com_par_check = new Regex(@"\w+\W{1}\d+\W{1}");
                    Regex Com_clear_check = new Regex(@"\w+\W{1}\W{1}");
                    

                    
                        if (Com_clear_check.IsMatch(subsub))
                        {
                            subsub = subsub.Trim(new char[] { '(', ')', ' ' });
                            _Body[Cmd_cnt] = subsub;
                            _Param[Cmd_cnt] = "NO_PARAM";
                            Cmd_cnt++;
                        }

                        else
                        {
                            if (Com_par_check.IsMatch(subsub))
                            {
                                string[] Sub_Command = subsub.Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                _Body[Cmd_cnt] = Sub_Command[0];
                                _Param[Cmd_cnt] = Sub_Command[1];
                                Cmd_cnt++;
                            }
                            else 
                            {
                                throw new InvalidCommandFormat();
                            }
                        }

                }

                Body = _Body;
                Param = _Param;




        }







        static void Main(string[] args)
        {
            string Command;
            bool Bad_except_catcher = false;
            Model.vRes_s_buf = Model.vRes_s;


            //buffering added
            /////////////////////////////////////////////////////////////////////////////////////////
            
            while (!SetUp.On()) ;
            do
            {
                if(Bad_except_catcher == true)
                {
                    Model.vRes_s = Model.vRes_s_buf; //откат до состояния получения команды
                    Bad_except_catcher = false;
                }
                else
                {
                    Model.vRes_s_buf = Model.vRes_s; 
                    File.WriteAllLines(SetUp.Path, Model.vRes_s_buf);//сохранение модели
                    Bad_except_catcher = false;

                }


                    Console.WriteLine("Введите команду:");
                    Command = Console.ReadLine();

                    ////////////////////////////////////////////////////////////////////////////////////////
                
                    Commander test;
                    try
                    {
                        test = new Commander(Command);
                    }
                    catch (InvalidCommandFormat) 
                    { 
                        Console.WriteLine("Недопустимая цепочка команд.");
                        Bad_except_catcher = true;
                    }
                    finally
                    {
                        test = new Commander(Command);
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////


                    if (Bad_except_catcher == false)
                    {


                        for (int i = 0; i < test.Cmd_cnt; i++)
                        {
                            Single_command Sub_command = test.Single_command_returner(i);
                            try
                            {
                                if (Sub_command.bdy == "REQUEST") Console.WriteLine(Model.Request());
                                if (Sub_command.bdy == "OCCUPY")
                                {
                                    
                                    Model.Occupy(Sub_command.prm);
                                    Console.WriteLine("Ресурс стал занятым.");
                                };
                                if (Sub_command.bdy == "FREE")
                                {
                                    Model.Free(Sub_command.prm);
                                    Console.WriteLine("Ресурс освобождён.");
                                };
                            }
                            catch (OverflowException) { Console.WriteLine("Такого ресурса нет."); Bad_except_catcher = true; }
                            catch (FormatException) { Console.WriteLine("Такого ресурса нет."); Bad_except_catcher = true; }
                            catch (ResIdInvalid) { Console.WriteLine("Такого ресурса нет."); Bad_except_catcher = true; }
                            catch (ResWasFree) { Console.WriteLine("Ресурс был свободен."); }
                            catch (ResAreBusy) { Console.WriteLine("Все ресурсы заняты."); }
                            catch (ResIsBusy) { Console.WriteLine("ресурс уже занят."); Bad_except_catcher = true; }
                        }
                    }
            }
            while (Command != "");

        }

    }
}

}