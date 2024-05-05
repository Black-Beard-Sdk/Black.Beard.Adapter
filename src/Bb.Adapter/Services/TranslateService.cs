using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using System.Globalization;

namespace Bb.Adapter.Services
{


  


    [ExposeClass("Service", ExposedType = typeof(ITranslateService), LifeCycle = IocScopeEnum.Singleton)]
    public class TranslateService : ITranslateService
    {


        public TranslateService(/*TranslateServiceDataAccess dataAccess*/)
        {

            //DataAccess = dataAccess;

        }

        //public TranslateServiceDataAccess DataAccess { get; }


        public string Translate(TranslatedKeyLabel key, params TranslatedKeyLabel[] arguments)
        {
            return Translate(CultureInfo.CurrentUICulture, key);
        }

        public CultureInfo[] AvailableCultures => new CultureInfo[] { Thread.CurrentThread.CurrentUICulture }; // DataAccess.AvailableCultures;

        ITranslateContainer ITranslateService.Container { get; set; }

        public string Translate(CultureInfo culture, TranslatedKeyLabel label, params TranslatedKeyLabel[] arguments)
        {

            //if (!label.IsNotValidKey)
            //{

            //    if (datas == null)
            //        lock (_lock)
            //            if (datas == null)
            //            {
            //                datas = new TranslateContainer(string.Empty);
            //                var d = DataAccess.GetAll().ToList();
            //                datas.Sort(d);
            //            }

            //    TranslateContainerResult r;

            //    if ((r = datas.Resolve(DataAccess.SplitPath(label.Path), label.Key, culture, 0, out TranslateServiceDataModel result)) != TranslateContainerResult.Resolved)
            //        lock (_lock)
            //            if ((r = datas.Resolve(DataAccess.SplitPath(label.Path), label.Key, culture, 0, out result)) != TranslateContainerResult.Resolved)
            //                foreach (var item in label.Datas)
            //                {
            //                    var result1 = DataAccess.GetNew(label.Path, label.Key);
            //                    result1.Culture = item.Value.Culture;
            //                    result1.Value = item.Value.Value ?? label.DefaultDisplay;
            //                    var sorted = datas.Sort(result1);
            //                    if (result1.Culture.IetfLanguageTag == culture.IetfLanguageTag)
            //                        result = result1;
            //                }


            //    foreach (var cultureItem in this.AvailableCultures)
            //        if ((r = datas.Resolve(DataAccess.SplitPath(label.Path), label.Key, cultureItem, 0, out result)) != TranslateContainerResult.Resolved)
            //            lock (_lock)
            //                if ((r = datas.Resolve(DataAccess.SplitPath(label.Path), label.Key, cultureItem, 0, out result)) != TranslateContainerResult.Resolved)
            //                    foreach (var item in label.Datas)
            //                    {
            //                        var result1 = DataAccess.GetNew(label.Path, label.Key);
            //                        result1.Culture = cultureItem;
            //                        result1.Value = item.Value.Value ?? label.DefaultDisplay;
            //                        var sorted = datas.Sort(result1);
            //                        if (result1.Culture.IetfLanguageTag == culture.IetfLanguageTag)
            //                            result = result1;
            //                    }


            //    if (result != null)
            //        return result.Value;

            //    else
            //    {

            //    }

            //}

            var result = label.DefaultDisplay;

            if (arguments != null && arguments.Length > 0)
                result = string.Format(result, arguments.Select(c => this.Translate(c)).ToArray());

            return result;

        }

        //private TranslateContainer datas;
        private volatile object _lock = new object();

    }


}
