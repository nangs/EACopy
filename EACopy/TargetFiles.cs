using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EACopy
{
    public class TargetFiles
    {
        public string FileName { get; set; }

        public List<string> TargetList 
        {
            get
            {
                return this._targetList;
            }
            set
            {
                this._targetList = value;
            } 
        }

        private List<string> _targetList;

        public void Load()
        {
            _targetList = new List<string>();

            using (StreamReader sr = File.OpenText(FileName))
            {
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    _targetList.Add(line);
                }
            }
        }
    }
}
