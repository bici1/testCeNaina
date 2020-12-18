using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsMobile
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Application.Run(new ProductGrid());
        }
    }
}