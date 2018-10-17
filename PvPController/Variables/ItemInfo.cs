using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Variables {
    public class ItemInfo {
        public ConcurrentDictionary<string, object> data = new ConcurrentDictionary<string, object>();
        public int id { get { return GetData<int>("ID"); } }
        public string name { get { return GetData<string>("Name"); } }
        public int damage { get { return GetData<int>("Damage"); } }
        public int shoot { get { return GetData<int>("Shoot"); } }
        public bool isShootModded { get { return GetData<bool>("IsShootModded"); } }
        public float shootSpeed { get { return GetData<float>("ShootSpeed"); } }
        public float knockback { get { return GetData<float>("Knockback"); } }
        public int defense { get { return GetData<int>("Defense"); } }
        public int inflictBuffID { get { return GetData<int>("InflictBuffID"); } }
        public int inflictBuffDuration { get { return GetData<int>("InflictBuffDuration"); } }
        public int receiveBuffID { get { return GetData<int>("ReceiveBuffID"); } }
        public int receiveBuffDuration { get { return GetData<int>("ReceiveBuffDuration"); } }
        public BuffDuration debuff { get { return new BuffDuration(inflictBuffID, inflictBuffDuration); } }
        public BuffDuration selfBuff { get { return new BuffDuration(receiveBuffID, receiveBuffDuration); } }

        public ItemInfo() {
            SetData<int>("ID", -1);
            SetData<string>("Name", "");
            SetData<int>("Damage", 0);
            SetData<int>("Shoot", -1);
            SetData<bool>("IsShootModded", false);
            SetData<int>("ShootSpeed", -1);
            SetData<float>("Knockback", 0);
            SetData<int>("Defense", 0);
            SetData<int>("InflictBuffID", 0);
            SetData<int>("InflictBuffDuration", 0);
            SetData<int>("ReceiveBuffID", 0);
            SetData<int>("ReceiveBuffDuration", 0);
        }

        public void SetData<T>(string key, T value) {
            if (!data.TryAdd(key, value)) {
                data.TryUpdate(key, value, data[key]);
            }
        }

        public T GetData<T>(string key) {
            object obj;
            if (!data.TryGetValue(key, out obj)) {
                return default(T);
            }

            return (T)obj;
        }

        public Type FindType(string key) {
            object obj;
            if (!data.TryGetValue(key, out obj)) {
                return default(Type);
            }

            return obj.GetType();
        }
    }

    public class BuffDuration {
        public int buffid { get; set; }
        public int buffDuration { get; set; }

        public BuffDuration(int buffid, int buffDuration) {
            this.buffid = buffid;
            this.buffDuration = buffDuration;
        }
    }
}
