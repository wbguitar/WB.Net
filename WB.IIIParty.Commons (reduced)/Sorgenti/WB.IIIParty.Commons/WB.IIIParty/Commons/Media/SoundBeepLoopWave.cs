// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Papi Rudy
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WB.IIIParty.Commons.Media
{
    /// <summary>
    /// Oggetto per la creazione di un allarme sonoro, con file Wave e menmorizzazione del'ultimo stato di eseguzione.
    /// </summary>
    public class SoundBeepLoopWaveMemoryState : ISoundBeepLoop, IDisposable
    {
        #region Field

        /// <summary>
        /// Nome del file su cui memorizzare lo stato.
        /// </summary>
        private string fileName = "SoundBeepLoopMemoryWaveStateObject";
        /// <summary>
        /// Oggetto per la creazione di un allarme sonoro.
        /// </summary>
        private SoundBeepLoopWave soundBeepLoopWave;
        /// <summary>
        /// IMessageLog
        /// </summary>
        private WB.IIIParty.Commons.Logger.IMessageLog messageLog;

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="_pathWave">Percorso del file wave da riprodurre, se null o stringa vuota usa il file di default.</param>        
        /// <param name="_delayBeep">Ripete il suono ogni n millisecondi.</param>
        /// <param name="_fileName">Nome del file senza estensione e path.</param>
        /// <param name="_messageLog">IMessageLog</param>
        public SoundBeepLoopWaveMemoryState(string _pathWave, TimeSpan _delayBeep, string _fileName, WB.IIIParty.Commons.Logger.IMessageLog _messageLog)
        {
            this.messageLog = _messageLog;

            try
            {

            this.soundBeepLoopWave = new SoundBeepLoopWave(_pathWave, _delayBeep);

            if (_fileName != null)
            {
                if (_fileName != "")
                    this.fileName = _fileName;
            }
                //Se c'? carica l'oggetto salvato con lo stato salvato
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
                SoundLoopState soundLoopState = (SoundLoopState)serializer.Deserialize(fs);
                fs.Dispose();

                //In base all'ultimo stato memorizzato reimposto l'oggetto.
                if (soundLoopState.IsActive)
                    this.soundBeepLoopWave.Enable();
                else
                    this.soundBeepLoopWave.Disable();
            }
            catch (Exception ex)
            {
                if(this.messageLog!=null)
                this.messageLog.Log(WB.IIIParty.Commons.Logger.LogLevels.Error, this, ex.ToString());
            }
            finally
            {
                try
                {
                    SoundLoopState soundLoopState = new SoundLoopState();
                    soundLoopState.IsActive = true;
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                    System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                    serializer.Serialize(fs, soundLoopState);
                    fs.Dispose();
                }
                catch (Exception ){ }
            }
        }
        /// <summary>
        /// Costruttore. Frenquenza suono=600, durata=100, ripetuto ogni 3 sec.
        /// </summary>
        /// <param name="_fileName">Nome del file senza estensione e path.</param>
        /// <param name="_messageLog">IMessageLog</param>
        public SoundBeepLoopWaveMemoryState(string _fileName, WB.IIIParty.Commons.Logger.IMessageLog _messageLog)
        {
            this.messageLog = _messageLog;

            this.soundBeepLoopWave = new SoundBeepLoopWave();

            if (_fileName != null)
            {
                if (_fileName != "")
                    this.fileName = _fileName;
            }
            try
            {
                //Se c'? carica l'oggetto salvato con lo stato salvato
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                SoundLoopState soundLoopState = (SoundLoopState)serializer.Deserialize(fs);
                fs.Dispose();

                //In base all'ultimo stato memorizzato reimposto l'oggetto.
                if (soundLoopState.IsActive)
                    this.soundBeepLoopWave.Enable();
                else
                    this.soundBeepLoopWave.Disable();

            }
            catch (Exception ex)
            {
                if (this.messageLog != null)
                this.messageLog.Log(WB.IIIParty.Commons.Logger.LogLevels.Error, this, ex.ToString());
            }
            finally
            {
                try
                {
                    SoundLoopState soundLoopState = new SoundLoopState();
                    soundLoopState.IsActive = true;
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                    System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                    serializer.Serialize(fs, soundLoopState);
                    fs.Dispose();
                }
                catch (Exception 
                    ) { }
            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.soundBeepLoopWave.Dispose();
        }

        #endregion

        #region ISoundBeepLoop Members

        void ISoundBeepLoop.Play()
        {
            this.soundBeepLoopWave.Play();
        }

        void ISoundBeepLoop.Stop()
        {
            this.soundBeepLoopWave.Stop();
        }

        void ISoundBeepLoop.EnableAudio()
        {
            this.soundBeepLoopWave.Enable();
            try
            {
                SoundLoopState soundLoopState = new SoundLoopState();
                soundLoopState.IsActive = true;
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                serializer.Serialize(fs, soundLoopState);
                fs.Dispose();
            }
            catch (Exception ex)
            {
                if (this.messageLog != null)
                this.messageLog.Log(WB.IIIParty.Commons.Logger.LogLevels.Error, this, ex.ToString());
            }
        }

        void ISoundBeepLoop.DisableAudio()
        {
            this.soundBeepLoopWave.Disable();
            try
            {
                SoundLoopState soundLoopState = new SoundLoopState();
                soundLoopState.IsActive = false;
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                serializer.Serialize(fs, soundLoopState);
                fs.Dispose();
            }
            catch (Exception ex)
            {
                if (this.messageLog != null)
                this.messageLog.Log(WB.IIIParty.Commons.Logger.LogLevels.Error, this, ex.ToString());
            }
        }

        bool ISoundBeepLoop.IsActive
        {
            get { return this.soundBeepLoopWave.IsActive; }
        }

        void ISoundBeepLoop.Dispose()
        {
            Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Oggetto per la creazione di un allarme sonoro.
    /// </summary>
    public class SoundBeepLoopWave : IDisposable
    {
        #region Private Field

        /// <summary>
        /// Oggetto SoundPlayer.
        /// </summary>
        private System.Media.SoundPlayer soundPlayer;
        /// <summary>
        /// Path di default del file wave.
        /// </summary>
        private string pathWave = @"\windows\media\chimes.wav";
        
        /// <summary>
        /// Ripete il suono ogni n millisecondi.
        /// </summary>
        private int delayBeep = 3000;

        private bool stopPlaySound = false;
        /// <summary>
        /// Thread eseguzione del sound.
        /// </summary>
        private Thread workerThread;
        /// <summary>
        /// Determina se il controllo ? abilitato o disabilitato per l'eseguzione.
        /// </summary>
        private bool isActive;
        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile bool shouldStop = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_pathWave">Percorso del file wave da riprodurre, se null o stringa vuota usa il file di default.</param>
        /// <param name="_delayBeep">Ripete il suono ogni n millisecondi.</param>
        public SoundBeepLoopWave(string _pathWave, TimeSpan _delayBeep)
        {
            if ((_pathWave != null) && (_pathWave != ""))
            {
                this.pathWave = _pathWave;
            }

            this.soundPlayer = new System.Media.SoundPlayer();
            this.soundPlayer.SoundLocation = this.pathWave;

            this.delayBeep = System.Convert.ToInt32(_delayBeep.TotalMilliseconds);
        }

        /// <summary>
        /// Costruttore. Frenquenza suono=600, durata=100, ripetuto ogni 3 sec.
        /// </summary>
        public SoundBeepLoopWave()
        {
            //Per default imposto l'audio ad attivo.
            this.Enable();

            this.soundPlayer = new System.Media.SoundPlayer();
            //Imposta il file di default.
            this.soundPlayer.SoundLocation = this.pathWave;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Se in stato di play esegue il suono.
        /// </summary>
        private void SoundLoop()
        {
            try
            {
                while (this.shouldStop)
                {
                    if (this.stopPlaySound)
                    {
                        this.soundPlayer.Play();
                    }
                    System.Threading.Thread.Sleep(delayBeep);
                }
            }
            catch (ThreadAbortException) { }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Interrompe il suono
        /// </summary>
        public void Stop()
        {
            this.stopPlaySound = false;
            this.soundPlayer.Stop();
        }
        /// <summary>
        /// Esegue il suono.
        /// </summary>
        public void Play()
        {
            this.stopPlaySound = true;
        }
        /// <summary>
        /// Attiva il thread per l'eseguzione del beep.
        /// </summary>
        public void Enable()
        {
            this.shouldStop = true;
            this.workerThread = new Thread(SoundLoop);

            // Start the worker thread.
            this.workerThread.Start();

            this.isActive = true;
        }
        /// <summary>
        /// Disabilit? il thread.
        /// </summary>
        public void Disable()
        {
            this.shouldStop = false;
            this.soundPlayer.Stop();
            if (this.workerThread != null)
            {
                if (this.workerThread.IsAlive)
                {
                    this.workerThread.Abort();
                }
            }
            this.isActive = false;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get { return this.isActive; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Disable();
        }

        #endregion
    }
}
