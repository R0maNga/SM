﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.Input.BasketToGameInput
{
    public class UpdateGameToBasketInput
    {
        public int Id { get; set; }
        
        public int BasketId { get; set; }
        public int GameId { get; set; }
    }
}
