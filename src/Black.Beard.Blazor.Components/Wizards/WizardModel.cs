using Bb.NewModules;
using System.ComponentModel;

namespace Bb.Wizards
{


    public class WizardModel : INotifyPropertyChanged
    {

        public WizardModel()
        {
            this._pages = new List<WizardPage>();
            this._index = 0;
        }

        public string Title { get; internal set; }


        public void Previous()
        {
            var index = this._pages.IndexOf(this.CurrentPage);
            if (index > 0)
            {
                _index--;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
                Wizard.Content = this;
            }

        }

        public void Next()
        {
            var index = this._pages.IndexOf(this.CurrentPage);
            if (index < this._pages.Count - 1)
            {
                _index++;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
                
            }
        }

        public void GoTo(int index)
        {
            if (index >= 0 && index < this._pages.Count)
                _index = index;
        }
                

        internal bool DisableCanPrevious()
        {
            var result = _index > 0;
            return !result;
        }

        internal bool DisableCanNext()
        {

            var result = _index < this._pages.Count - 1;

            if (result)
                result = _pages[_index].IsValid();

            return !result;

        }

        internal bool DisableCanValidate()
        {

            var result = _index == this._pages.Count - 1;

            if (result)
                result = _pages[_index].IsValid();

            return !result;
        }

        internal bool DisableCanGoto(int index)
        {
            var result = index >= 0 && index < this._pages.Count;
            return result;
        }


        

        public WizardModel AddPage(object model, string title = null, Action<WizardPage> action = null)
        {

            var page = new WizardPage()
            {
                Parent = this,
                Title = title ?? "Page " + (this._pages.Count + 1),
                Model = model,
            };

            if (action != null)
                action(page);

            this._pages.Add(page);



            return this;
        }

        public IEnumerable<WizardPage> Pages => this._pages;

        internal WizardPage CurrentPage { get => _pages[_index]; }

        public UIWizard Wizard { get; internal set; }


        private int _index;
        private List<WizardPage> _pages;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
       
}
