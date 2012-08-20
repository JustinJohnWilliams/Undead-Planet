using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class ItemRepository : IItemRetriever, IUserItemRetriever, IUserItemSaver, ISafeHouseItemSaver
    {
        private string _connectionString;
        private static Guid _tentId = new Guid("7CC21B96-B3AF-4257-B3B7-EA7113844654");

        public ItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region "IItemRetriever Implementation"

        IItem IItemRetriever.GetItemById(Guid itemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.GetItemById(itemId);
            }
        }

        bool IItemRetriever.ItemExists(Guid itemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ItemExists(itemId);
            }
        }

        List<IItem> IItemRetriever.GetAllBelowPrice(int price)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ItemDtos
                                  .Where(i => i.Price <= price)
                                  .Cast<IItem>()
                                  .ToList();
            }
        }

        #endregion

        #region IUserItemRetriever Implementation

        List<IItem> IUserItemRetriever.GetUserItems(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                List<IItem> items = (from userItems in dataContext.UserItemDtos
                                     join item in dataContext.ItemDtos
                                     on userItems.ItemId equals item.Id
                                     where userItems.UserId == userId
                                     select item).Cast<IItem>().ToList();

                return items;
            }
        }

        bool IUserItemRetriever.UserHasItem(Guid userId, Guid itemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.UserItemExists(userId, itemId);
            }
        }

        Guid IUserItemRetriever.GetUserItemId(Guid userId, Guid itemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.GetUserItemByUserIdAndItemId(userId, itemId).UserItemId;
            }
        }

        int IUserItemRetriever.GetUserItemCount(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.UserItemDtos.Where(c => c.UserId == userId).Count();
            }
        }

        #endregion

        #region IUserItemSaver Implementation

        void IUserItemSaver.SaveUserItem(Guid userId, Guid itemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.UserItemDtos.InsertOnSubmit(new UserItemDto
                                                        {
                                                            UserItemId = Guid.NewGuid(),
                                                            ItemId = itemId,
                                                            UserId = userId
                                                        });

                dataContext.SubmitChanges();
            }
        }

        void IUserItemSaver.RemoveUserItem(Guid userItemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.UserItemDtos.DeleteAllOnSubmit(
                                    dataContext.UserItemDtos.Where(c => c.UserItemId == userItemId));

                dataContext.SubmitChanges();
            }
        }

        #endregion

        #region ISafeHouseItemSaver Implementation

        void ISafeHouseItemSaver.SaveItemInSafeHouse(Guid safeHouseId, Guid itemId, Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.SafeHouseItemDtos
                    .InsertOnSubmit(new SafeHouseItemDto
                                        {
                                            Id = Guid.NewGuid(),
                                            ItemId = itemId,
                                            SafeHouseId = safeHouseId,
                                            UserId = userId
                                        });

                dataContext.SubmitChanges();
            }
        }

        void ISafeHouseItemSaver.RemoveSafeHouseItem(Guid safeHouseUserItemId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.SafeHouseItemDtos.DeleteAllOnSubmit(
                                    dataContext.SafeHouseItemDtos.Where(c => c.Id == safeHouseUserItemId));

                dataContext.SubmitChanges();
            }
        }

        #endregion


        void IUserItemSaver.AddTent(Guid userId, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                (this as IUserItemSaver).SaveUserItem(userId, _tentId);    
            }
        }
    }
}