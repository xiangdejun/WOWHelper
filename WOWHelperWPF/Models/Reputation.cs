using System;
using System.Collections.Generic;

namespace WOWHelperWPF.Models
{
    public class Reputaion
    {
        public string Name { get; set; }
        public string Score { get; set; }
        public string Level { get; set; }

        public int Current
        {
            get
            {
                int result = 0;

                if (!String.IsNullOrEmpty(this.Score))
                {
                    var arr = this.Score.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(arr[0]))
                    {
                        result = int.Parse(arr[0].Trim());
                    }
                }

                return result;
            }
        }

        public int Max
        {
            get
            {
                int result = 0;

                if (!String.IsNullOrEmpty(this.Score))
                {
                    var arr = this.Score.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(arr[1]))
                    {
                        result = int.Parse(arr[1].Trim());
                    }
                }

                return result;
            }
        }

        private bool _showProgress = true;
        public bool ShowProgress
        {
            get
            {
                return _showProgress;
            }
            set
            {
                _showProgress = value;
            }
        }

        public List<Reputaion> RepList
        {
            get
            {
                if (_repList == null)
                    _repList = new List<Reputaion>();
                return _repList;
            }
        }
        private List<Reputaion> _repList;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Score))
                return Name;
            return Name + "[" + Score + "]";
        }
    }

}
