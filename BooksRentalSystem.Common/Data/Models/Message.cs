using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BooksRentalSystem.Common.Data.Models
{
    public class Message
    {
        private string _serializedData;

        public Message(object data)
            => Data = data;

        private Message()
        {
        }

        public int Id { get; private set; }

        public Type Type { get; private set; }

        public bool Published { get; private set; }

        public void MarkAsPublished() => Published = true;

        [NotMapped]
        public object Data
        {
            get => JsonConvert.DeserializeObject(_serializedData, Type,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

            set
            {
                Type = value.GetType();

                _serializedData = JsonConvert.SerializeObject(value,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            }
        }
    }
}
