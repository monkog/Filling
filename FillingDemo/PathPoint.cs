using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FillingDemo
{
    class PathPoint
    {
        public PathPoint(Point point, byte type)
        {
            m_point = point;
            m_type = type;
        }

        public Point m_point { get; set; }
        public byte m_type { get; set; }
    }
}
