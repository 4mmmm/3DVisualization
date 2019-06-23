using System;
using System.Collections.Generic;

namespace HELONGWEI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始收录学生信息!");
            Console.Write("请输入你想收录学生信息的总人数:");
            int sum = Convert.ToInt32(Console.ReadLine());

            Student student;
            List<Student> students = new List<Student>();

            for (int i=0;i<sum;i++)
            {
                student = new Student();
                student.Num = i + 1;
                Console.WriteLine("第{0}个学生信息：",student.Num);
                student.GetInfo();
                students.Add(student);
            }

            Console.WriteLine("已收录{0}名学生信息！",sum);
            foreach (var v in students)
                v.ShowInfo();

            Console.ReadLine();
        }
    }
    class Student
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Num { get; set; }
        public Student() { }

        public void GetInfo()
        {
            Console.Write("请输入第{0}名学生的姓名：",this.Num);
            this.Name = Convert.ToString(Console.ReadLine());
            Console.Write("请输入第{0}名学生的学号：", this.Num);
            this.Id = Convert.ToInt32(Console.ReadLine());
            
        }

        public void ShowInfo()
        {
            Console.Write("第{0}名学生的姓名是：",this.Num);
            Console.WriteLine(this.Name);
            Console.Write("第{0}名学生的学号是：", this.Num);
            Console.WriteLine(this.Id);
        }
    }
}