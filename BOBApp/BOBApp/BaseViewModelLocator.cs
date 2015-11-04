using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using BOBApp.ViewModels;
using BOBApp.Models;

namespace BOBApp
{
    public class BaseViewModelLocator //Gaat de views aan de viewmodels koppelen. Moet aangeroepen worden (zie app.xaml)
    {
        public static Login USER;
        public BaseViewModelLocator()
        {//Zorgen dat je geen harde verbinding hebt tussen 2 dingen, objecten injecteren in containers.

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);//In onze Ioc container zal een bepaalde view zitten en gekoppeld worden.

            //VMs registreren.
            SimpleIoc.Default.Register<BestemmingenVM>();
            SimpleIoc.Default.Register<FeestjesOverzichtVM>();
            SimpleIoc.Default.Register<LoginVM>();
            SimpleIoc.Default.Register<ProfielVM>();
            SimpleIoc.Default.Register<PuntenVM>();
            SimpleIoc.Default.Register<RegisterVM>();
            SimpleIoc.Default.Register<RittenVM>();
            SimpleIoc.Default.Register<VindRitVM>();
            SimpleIoc.Default.Register<VindRit1VM>();
            SimpleIoc.Default.Register<ZoekVriendenVM>();




        }

        public LoginVM LoginVM
        {
            get { return ServiceLocator.Current.GetInstance<LoginVM>(); }//Viewmodels opvragen
        }

        public RittenVM RittenVM
        {
            get { return ServiceLocator.Current.GetInstance<RittenVM>(); }//Viewmodels opvragen
        }
    }
}
