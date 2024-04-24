using Bb.ComponentModel.Attributes;
using MudBlazor;
using System.ComponentModel;

namespace Bb.Wizards
{


    [ExposeClass("Service", ExposedType = typeof(WizardModel), LifeCycle = IocScopeEnum.Transiant)]
    public class WizardModel : INotifyPropertyChanged
    {

        public WizardModel(IDialogService dialogService)
        {

            this.DialogService = dialogService;

            this._pages = new List<WizardPage>();
            this._index = 0;
        }

        public string Title { get; set; }

        public WizardModel SetTitle(string title)
        {
            this.Title = title;
            return this;
        }

        public WizardModel SetModel(object model)
        {
            this._model = model;
            return this;
        }

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

        public WizardModel AddPage(string title = null, Action<WizardPage> action = null)
        {

            if (this._model == null)
                throw new InvalidOperationException("Model is not defined");

            return AddPage(this._model, title, action);

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

        public int Index { get => _index; }

        internal WizardPage CurrentPage { get => _pages[_index]; }

        public UIWizard Wizard { get; internal set; }

        public WizardModel Show(DialogOptions options = null)
        {

            var b = new DialogParameters
            {
                { "Content", this }
            };

            if (options == null)
                options = new DialogOptions()
                {
                    MaxWidth = MaxWidth.Medium,
                    FullWidth = true,
                    CloseOnEscapeKey = true,
                    NoHeader = false,
                    Position = DialogPosition.Center,
                };

             DialogService.ShowMessageBox(new MessageBoxOptions() { Message = "Ok", YesText = "Ok" });
            //_reference = DialogService.Show<UIWizard>("Options Dialog", b, options);

            return this;

        }

        internal void Exit(WizardResult result)
        {
            if (_close != null)
                _close(this, result);
        }

        public WizardModel ExecuteOnClose(Action<WizardModel, WizardResult> action)
        {
            this._close = action;
            return this;
        }

        public T GetModel<T>()
        {
            return (T)this._model;
        }

        public T GetModel<T>(int page)
        {
            return (T)this._pages[page].Model;
        }

        public IDialogService DialogService { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Action<WizardModel, WizardResult> _close;
        private int _index;
        private List<WizardPage> _pages;
        private object _model;
        private IDialogReference _reference;


    }

    public enum WizardResult
    {
        Cancel,
        Ok,
    }

}
