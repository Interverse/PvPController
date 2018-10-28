namespace PvPController.Variables {
    public class BuffInfo {
        public int BuffId { get; set; }
        public int BuffDuration { get; set; }

        public BuffInfo(int buffId, int buffDuration) {
            BuffId = buffId;
            BuffDuration = buffDuration;
        }
    }
}
