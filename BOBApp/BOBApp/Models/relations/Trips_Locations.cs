﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models.relations
{
    public class Trips_Locations
    {
        public int Trips_ID { get; set; }
        public string Location { get; set; }
        public DateTime Added { get; set; }
    }
}
