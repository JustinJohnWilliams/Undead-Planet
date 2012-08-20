using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UndeadEarth.Silverlight.Model;

namespace UndeadEarth.Silverlight.Presenters
{
    public class LevelUpPresenter
    {
        public interface IView
        {
            void PopulateNewLevel(int newLevel);
            void PopulateNewAttack(int newAttack);
            void PopulateNewEnergy(int newEnergy);
            void PopulateNewZombie(long zombiesNeededForNextLevel);
            void PopulateNewItemSlot(int newItemSlot);
        }

        private IView _view;
        private GameContext _gameContext;

        /// <summary>
        /// Initializes a new instance of the LevelUpPresenter class.
        /// </summary>
        public LevelUpPresenter(IView view, GameContext gameContext)
        {
            _view = view;
            _gameContext = gameContext;
        }

        /// <summary>
        /// Gets level information for user.
        /// </summary>
        public void RequestLevelInfo()
        {
            _view.PopulateNewLevel(_gameContext.UserLevel.CurrentLevel);
            _view.PopulateNewAttack(_gameContext.UserAttackPower);
            _view.PopulateNewEnergy(_gameContext.UserEnergy.TotalEnergy);
            _view.PopulateNewItemSlot(_gameContext.UserMaxItems);
            _view.PopulateNewZombie(_gameContext.UserLevel.ZombiesNeededForNextLevel - _gameContext.UserLevel.ZombiesKilledLastLevel);
        }
    }
}
