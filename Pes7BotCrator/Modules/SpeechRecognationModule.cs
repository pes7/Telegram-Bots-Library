using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using Pes7BotCrator.Type;

namespace Pes7BotCrator.Modules
{
    class SpeechRecognationModule : Module
    {
        private SpeechRecognitionEngine sr;
        private DictationGrammar dictationGrammar;
        public SpeechRecognationModule() : base("SpeechRecognationModule",typeof(SpeechRecognationModule))
        {
            rec();
        }
        public void rec()
        {
            dictationGrammar = new DictationGrammar();
            sr.LoadGrammar(dictationGrammar);
            sr.SetInputToDefaultAudioDevice();//микрофон
            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(spechrecognized);//событие речь распознана
            sr.RecognizeAsync(RecognizeMode.Multiple);//начинаем распознование
        }

        private void spechrecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
        }
    }
}
