namespace VaporStore.DataProcessor
{
	using System;
    using System.Linq;
    using Data;

	public static class Bonus
	{
		public static string UpdateEmail(VaporStoreDbContext context, string username, string newEmail)
		{
            var user = context.Users.SingleOrDefault(e => e.Username == username);

            if (user == null)
            {
                return $"User {username} not found";
                //throw new ArgumentException($"User {username} not found");
            }

            var email = context.Users.Any(e => e.Email == newEmail);

            if (email)
            {
                return $"Email {newEmail} is already taken";
                //throw new ArgumentException($"Email {newEmail} is already taken");
            }

            user.Email = newEmail;
            context.SaveChanges();

            return $"Changed {username}'s email successfully";
		}
	}
}
