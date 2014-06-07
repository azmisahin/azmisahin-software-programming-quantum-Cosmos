namespace Cosmos
{
    public partial class Kuark:KuarkObject
    {
        
        #region Member
        private object _Momentus;

        public object Momentus
        {
            get { return _Momentus; }
            set { _Momentus = value; }
        }
        #endregion

        #region Constroctor
        /// <summary>
        /// Default Başlangıç fermiyon ailesi
        /// </summary>
        public Kuark()
        {
            SetSpinByAile(Aileleri.Fermiyon);
        }

        /// <summary>
        /// Kuark oluşturulur iken ailesi belirtilebilir
        /// </summary>
        /// <param name="aile"></param>
        public Kuark(Aileleri aile)
        {
            SetSpinByAile(aile);
        }

        #endregion
        
        #region Local Method
        private System.Threading.Thread life;

        private void Titre()
        {
            life = new System.Threading.Thread(TitremeHaraketi);
            life.Start();//Yaşam Başladı
        }

        private void TitremeHaraketi()
        {
            do
            {
                _Momentus = ((double)this.Spin) * (int)System.DateTime.Now.Ticks;
                System.Console.WriteLine(string.Format("Atomic Time : {0} - Aile : {1} - Momentus : {2}", AtomicTime.Now, System.DateTime.Now, this.Aile));
                System.Threading.Thread.Sleep(1000);

                var o = this.GetType();
            } while (true);
            
        }
        
        private void SetSpinByAile(Aileleri aile)
        {
            switch(aile)
            {
                default: this.Spin = (double)1 / 2; this.Aile = aile; break;
                case Aileleri.Fermiyon: this.Spin = (double)1 / 2; this.Aile = aile; break;
                case Aileleri.Lepton: this.Spin = (double)1; this.Aile = aile; break;
            }
            Titre();
        }
        #endregion
        
    }
}
