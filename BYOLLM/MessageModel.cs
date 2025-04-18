﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
{
    public record MessageModel
    {
        [JsonConstructor]
        public MessageModel( string text, string attachment)
        {
            Id = Guid.NewGuid().ToString();
            Text = text;
            Attachment = attachment;
        }


        public string Id { get; }
        public string Text { get; set; }
        public string Attachment { get; set; }
    }
}
