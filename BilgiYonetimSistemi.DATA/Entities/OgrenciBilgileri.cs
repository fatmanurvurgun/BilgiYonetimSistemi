﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgiYonetimSistemi.DATA
{
   public class OgrenciBilgileri
    {
        public int OgrenciID { get; set; }

        public string Adres { get; set; }

        public string Telefon { get; set; }

        public string TCNo { get; set; }

        public string OgrenciMail { get; set; }

        public bool MezunMu { get; set; }
        public string Fotograf { get; set; }

        public string Sifre { get; set; }

        public virtual Ogrenci BilgininOgrencisi { get; set; }
    }
}