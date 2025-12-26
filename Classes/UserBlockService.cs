using System;
using System.Linq;

namespace ekz.Classes
{
    public static class UserBlockService
    {
        public static bool IsUserBlockedForInactivity(ekz.Models.users user)
        {
            try
            {
                // Используйте свойства через параметр user
                if (user.LastLoginDate == null)
                    return false;

                if (user.IsBlocked == "1" || user.IsBlocked == "Y")
                    return true;

                if (user.BlockedUntil.HasValue && user.BlockedUntil > DateTime.Now)
                    return true;

                DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
                return user.LastLoginDate < oneMonthAgo;
            }
            catch
            {
                return false;
            }
        }

        public static void BlockUserForInactivity(int userId)
        {
            try
            {
                using (var context = new ekz.Models.qwsEntities1())
                {
                    var user = context.users.FirstOrDefault(u => u.ID == userId);
                    if (user != null)
                    {
                        user.IsBlocked = "1";
                        user.BlockedUntil = DateTime.Now.AddMonths(1);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка блокировки пользователя: {ex.Message}");
            }
        }

        public static void UnblockUser(int userId)
        {
            try
            {
                using (var context = new ekz.Models.qwsEntities1())
                {
                    var user = context.users.FirstOrDefault(u => u.ID == userId);
                    if (user != null)
                    {
                        user.IsBlocked = "0";
                        user.BlockedUntil = null;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка разблокировки пользователя: {ex.Message}");
            }
        }

        public static void CheckAllUsersForInactivity()
        {
            try
            {
                using (var context = new ekz.Models.qwsEntities1())
                {
                    var inactiveUsers = context.users
                        .Where(u => u.LastLoginDate != null &&
                                   u.LastLoginDate < DateTime.Now.AddMonths(-1) &&
                                   u.IsBlocked != "1")
                        .ToList();

                    foreach (var user in inactiveUsers)
                    {
                        user.IsBlocked = "1";
                        user.BlockedUntil = DateTime.Now.AddMonths(1);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка проверки неактивности: {ex.Message}");
            }
        }
    }
}