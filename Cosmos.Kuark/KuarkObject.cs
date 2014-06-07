namespace Cosmos
{
    public abstract class KuarkObject
    {
        #region Enum
        public enum Tipleri
        {
            Yukarı,
            Aşağı,
            Tılsım,
            Acayip,
            Üst,
            Alt
        }
        public enum Aileleri
        {
            Fermiyon,
            Lepton
        }
        public enum Sembolleri
        {
            q
        }
        #endregion

        #region Base Member
        private Tipleri _Tip;

        public Tipleri Tip
        {
            get { return _Tip; }
            set { _Tip = value; }
        }

        private Aileleri _Aile;

        public Aileleri Aile
        {
            get { return _Aile; }
            set { _Aile = value; }
        }

        private Sembolleri _Sembol;

        public Sembolleri Sembol
        {
            get { return _Sembol; }
            set { _Sembol = value; }
        }

        #endregion

        #region Member
        private object _ElektrikYuku;

        public object ElektrikYuku
        {
            get { return _ElektrikYuku; }
            set { _ElektrikYuku = value; }
        }

        private object _RenkYuku;

        public object RenkYuku
        {
            get { return _RenkYuku; }
            set { _RenkYuku = value; }
        }

        private object _Kutle;

        public object Kutle
        {
            get { return _Kutle; }
            set { _Kutle = value; }
        }

        private object _Spin;

        public object Spin
        {
            get { return _Spin; }
            set { _Spin = value; }
        }
        #endregion
      
    }
}
