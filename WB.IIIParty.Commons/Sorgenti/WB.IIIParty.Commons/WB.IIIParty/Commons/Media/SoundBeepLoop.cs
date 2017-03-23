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
    /// Oggetto per la creazione di un allarme sonoro con menmorizzazione del'ultimo stato di eseguzione.
    /// </summary>
    public class SoundBeepLoopMemoryState : ISoundBeepLoop, IDisposable
    {
        #region Field

        /// <summary>
        /// Nome del file su cui memorizzare lo stato.
        /// </summary>
        private string fileName = "SoundBeepLoopMemoryStateObject";
        /// <summary>
        /// Oggetto per la creazione di un allarme sonoro.
        /// </summary>
        private SoundBeepLoop soundBeepLoop;
        /// <summary>
        /// 
        /// </summary>
        private WB.IIIParty.Commons.Logger.IMessageLog messageLog;

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="_frequencyBeep">Frequenza del suono.</param>
        /// <param name="_durationBeep">Tempo di durata di ogni singolo suono.</param>
        /// <param name="_delayBeep">Ripete il suono ogni n millisecondi.</param>
        /// <param name="_fileName">Nome del file senza estensione e path.</param>
        /// <param name="_messageLog">IMessageLog</param>
        public SoundBeepLoopMemoryState(int _frequencyBeep, TimeSpan _durationBeep, TimeSpan _delayBeep, string _fileName, WB.IIIParty.Commons.Logger.IMessageLog _messageLog)
        {
            this.messageLog = _messageLog;

            this.soundBeepLoop = new SoundBeepLoop(_frequencyBeep, _durationBeep, _delayBeep);

            if (_fileName != null)
            {
                if (_fileName != "")
                    this.fileName = _fileName;
            }
            try
            {
                //Se c'? carica l'oggetto salvato con lo stato salvato
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
                SoundLoopState soundLoopState = (SoundLoopState)serializer.Deserialize(fs);
                fs.Dispose();

                //In base all'ultimo stato memorizzato reimposto l'oggetto.
                if (soundLoopState.IsActive)
                    this.soundBeepLoop.Enable();
                else
                    this.soundBeepLoop.Disable();
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
                catch (Exception ) { }
            }
        }
        /// <summary>
        /// Costruttore. Frenquenza suono=600, durata=100, ripetuto ogni 3 sec.
        /// </summary>
        /// <param name="_fileName">Nome del file senza estensione e path.</param>
        /// <param name="_messageLog">IMessageLog</param>
        public SoundBeepLoopMemoryState(string _fileName, WB.IIIParty.Commons.Logger.IMessageLog _messageLog)
        {
            this.messageLog = _messageLog;

            this.soundBeepLoop = new SoundBeepLoop();

            if (_fileName != null)
            {
                if (_fileName != "")
                    this.fileName = _fileName;
            }
            try
            {
                //Se c'? carica l'oggetto salvato con lo stato salvato
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SoundLoopState));
                System.IO.FileStream fs = new System.IO.FileStream(@".\" + this.fileName + ".xml", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
                SoundLoopState soundLoopState = (SoundLoopState)serializer.Deserialize(fs);
                fs.Dispose();

                //In base all'ultimo stato memorizzato reimposto l'oggetto.
                if (soundLoopState.IsActive)
                    this.soundBeepLoop.Enable();
                else
                    this.soundBeepLoop.Disable();

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
                catch (Exception ) { }
            }
        }

        #endregion        

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.soundBeepLoop.Dispose();
        }

        #endregion

        #region ISoundBeepLoop Members
        
        bool ISoundBeepLoop.IsActive
        {
            get { return this.soundBeepLoop.IsActive; }
        }

        void ISoundBeepLoop.Play()
        {
            this.soundBeepLoop.Play();
        }

        void ISoundBeepLoop.Stop()
        {
            this.soundBeepLoop.Stop();
        }

        void ISoundBeepLoop.EnableAudio()
        {
            this.soundBeepLoop.Enable();
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
            this.soundBeepLoop.Disable();

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

        void ISoundBeepLoop.Dispose()
        {
            Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Oggetto per la creazione di un allarme sonoro.
    /// </summary>
    public class SoundBeepLoop : IDisposable
    {
        #region Private Field

        /// <summary>
        /// Frequenza del suono.
        /// </summary>
        private int frequencyBeep = 600;
        /// <summary>
        /// Tempo di durata di ogni singolo suono.
        /// </summary>
        private int durationBeep = 100;
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
        /// <param name="_frequencyBeep">Frequenza del suono.</param>
        /// <param name="_durationBeep">Tempo di durata di ogni singolo suono.</param>
        /// <param name="_delayBeep">Ripete il suono ogni n millisecondi.</param>
        public SoundBeepLoop(int _frequencyBeep, TimeSpan _durationBeep, TimeSpan _delayBeep)
        {
            this.frequencyBeep = _frequencyBeep;
            this.durationBeep = System.Convert.ToInt32(_durationBeep.TotalMilliseconds);
            this.delayBeep = System.Convert.ToInt32(_delayBeep.TotalMilliseconds);
        }

        /// <summary>
        /// Costruttore. Frenquenza suono=600, durata=100, ripetuto ogni 3 sec.
        /// </summary>
        public SoundBeepLoop()
        {
            //Per default imposto l'audio ad attivo.
            this.Enable();
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
                        Console.Beep(frequencyBeep, durationBeep);
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