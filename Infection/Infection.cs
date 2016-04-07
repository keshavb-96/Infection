using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infection
{
    /// <summary>
    /// A user can be modelled using 4 attributes. His or her name, his or her coaches, all the students he or she is coaching and the current version of the website (expressed as an unsinged integer) they see.
    /// We assume that every User starts with the same version of the website - version #0
    /// </summary>
    class User
    {
        public string name;
        public List<User> coaches;
        public List<User> students;
        public uint versionNumber;

        public User(string _name)
        {
            name = _name;
            coaches = new List<User>();
            students = new List<User>();
            versionNumber = 0;
        }
    }

    class totalInfection
    {
        public Dictionary<string, User> users; // The entire coaching graph can be expressed as a list of users.

        public totalInfection(Dictionary<string, User> _users)
        {
            users = _users;
        }

        /// <summary>
        /// Method that infects the entire component of the coaching graph connected to the User root
        /// </summary>
        void spreadInfection(User root)
        {

            //First infecting all the students
            foreach (User student in root.students)
            {
                if (student != null && (root.versionNumber != student.versionNumber))
                    addFeature(student);
            }

            //Working our way up and infecting all coaches
            foreach (User coach in root.coaches)
            {
                if (coach != null && (root.versionNumber != coach.versionNumber))
                    addFeature(coach);
            }
        }

        /// <summary>
        /// Adds a feature to the website of the particular user. 
        /// </summary>
        public void addFeature(User u)
        {
            u.versionNumber += 1;
            spreadInfection(u);
        }

        /// <summary>
        /// Displays the version of each user
        /// </summary>
        public void displayVersion()
        {
            foreach(KeyValuePair<string, User> kvp in users)
            {
                Console.WriteLine("Name:" + kvp.Value.name + ", Site Version:" + kvp.Value.versionNumber);
            }
        }
    }

    class limitedInfection
    {
        int limit;
        public Dictionary<string, User> users;

        public limitedInfection(Dictionary<string, User> _users)
        {
            users = _users;
        }

        public void setLimit(int _limit)
        {
            limit = _limit;
        }


        public void addFeature(User u, ref int affectedCount)
        {
            int currentNumberAffected = affectedCount; // The affected count at the beginning of the recursive call
            List<User> connectedUsers = new List<User>();

            // Count the number of Users (connected to User u) whose version we are going to update
            foreach (User student in u.students)
            {
                if (u.versionNumber != student.versionNumber)
                {
                    affectedCount += 1;
                    connectedUsers.Add(student);
                }
            }

            foreach (User coach in u.coaches)
            {
                if (u.versionNumber != coach.versionNumber)
                {
                    affectedCount += 1;
                    connectedUsers.Add(coach);
                }
            }


            if (Math.Abs(affectedCount - limit) > Math.Abs(currentNumberAffected - limit))
                return;


            foreach(User cu in connectedUsers)
            {
                cu.versionNumber = u.versionNumber;
            }

            foreach(User cu in connectedUsers)
            {
                addFeature(cu, ref affectedCount);
            }
        }

        public void displayVersion()
        {
            foreach (KeyValuePair<string, User> kvp in users)
            {
                Console.WriteLine("Name:" + kvp.Value.name + ", Site Version:" + kvp.Value.versionNumber);
            }
        }
    }




    class Infection
    {
        static Dictionary<string, User> createUserGraph(string file)
        {
            Dictionary<string, User> users = new Dictionary<string, User>();
            List<string> userNames = new List<string>();
            int n;
            bool readingCoaches = true;

            using(StreamReader sr = new StreamReader(file))
            {
                string line;

                //Line 1
                line = sr.ReadLine().Trim();
                
                if(!int.TryParse(line, out n))
                {
                    Console.WriteLine("Incorrect File format");
                    return null;
                }

                //Line 2
                line = sr.ReadLine().Trim();

                userNames = line.Split(' ').ToList();
                foreach(string userName in userNames)
                {
                    User user = new User(userName);
                    users.Add(userName, user);
                }

                //Lines 3 through 3+n
                for(int i = 0; i< n; i++)
                {
                    readingCoaches = true;
                    line = sr.ReadLine().Trim();
                    string[] tmp = line.Split(' ');

                    if(tmp[0] == "$")
                    {
                        for(int j = 1; j<tmp.Length; j++)
                        {
                            users[userNames[i]].students.Add(users[tmp[j]]);
                        }
                    }

                    else if(tmp[0] == "#")
                    {
                        for(int j = 1; j<tmp.Length; j++)
                        {
                            if(tmp[j] == "$")
                            {
                                readingCoaches = false;
                                continue;
                            }

                            if(readingCoaches)
                                users[userNames[i]].coaches.Add(users[tmp[j]]);
                            else
                                users[userNames[i]].students.Add(users[tmp[j]]);
                        }
                    }

                }
            }
            return users;
        }

        static void Main(string[] args)
        {
            string file;
            int n;
            

            Console.WriteLine("Please enter the file path of the User graph that describes the coaching relation between every user");
            file = Console.ReadLine();
            Dictionary<string, User> users = createUserGraph(file);
            totalInfection TI = new totalInfection(users);
            limitedInfection LI = new limitedInfection(users);

            Console.WriteLine("Type 1 to test Total Infection");
            Console.WriteLine("Type 2 to test Limited Infection");
            Console.WriteLine("Type 0 to exit");

            n = int.Parse(Console.ReadLine());

            if(n == 1)
            {
                int a;
                bool isInt;
                Console.WriteLine("At any time enter 0 to exit");
                Console.WriteLine("At any time enter 1 to see each user's website version");
                Console.WriteLine("At any time enter 2 to add a feature to a particular user and thereby start an infection");
                while (true)
                {
                    string entry = Console.ReadLine();
                    isInt = int.TryParse(entry, out a);

                    if (isInt && a == 1)
                        TI.displayVersion();

                    else if (isInt && a == 2)
                    {
                        Console.WriteLine("Please enter the user's name at whom you would like to start the infection");
                        string user = Console.ReadLine();
                        TI.addFeature(users[user]);
                    }

                    else if (isInt && a == 0)
                        break;
                }
            }

            else if(n == 2)
            {
                int a;
                bool isInt;
                Console.WriteLine("At any time enter 0 to exit");
                Console.WriteLine("At any time enter 1 to see each user's website version");
                Console.WriteLine("At any time enter 2 to add a feature to a particular user and thereby start an infection");
                while (true)
                {
                    string entry = Console.ReadLine();
                    isInt = int.TryParse(entry, out a);

                    if (isInt && a == 1)
                        LI.displayVersion();

                    else if (isInt && a == 2)
                    {
                        Console.WriteLine("Please enter the user's name at whom you would like to start the infection");
                        string user = Console.ReadLine();
                        Console.WriteLine("Please enter the number of users you would ideally like to infect");
                        int num = int.Parse(Console.ReadLine());
                        LI.setLimit(num);
                        users[user].versionNumber += 1;
                        int initAffectedCount = 1;
                        LI.addFeature(users[user], ref initAffectedCount);
                    }

                    else if (isInt && a == 0)
                        break;
                }
            }

            else if(n == 0)
            {
                return;
            }
        }
    }
}
