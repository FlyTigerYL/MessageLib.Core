using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageLib.Core.ClassBean
{
    public abstract class Item
    {
        public string appid { get; set; }
        public string type { get; set; }
        public string body { get; set; }
        public string time { get; set; }
    }

    public class ErrorItem : Item
    {
        public ErrorItem() { }
        public ErrorItem(string appid, string body)
        {
            this.appid = appid;
            this.type = ItemType.error;
            this.body = body;
            this.time = DateTime.Now.ToString();
        }
    }

    public class NoticItem : Item
    {
        public NoticItem() { }
        public NoticItem(string appid, string body)
        {
            this.appid = appid;
            this.type = ItemType.notic;
            this.body = body;
            this.time = DateTime.Now.ToString();
        }
    }

    public class Message:Item
    {
        public Message() { }

        public Message(string appid, string body)
        {
            this.appid = appid;
            this.type = ItemType.info;
            this.body = body;
            this.time = DateTime.Now.ToString();
        }
    } 
}
