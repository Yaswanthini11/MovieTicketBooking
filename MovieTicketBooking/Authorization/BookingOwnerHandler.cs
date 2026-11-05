
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MovieTicketBooking.Models;
using MovieTicketBooking.Constants;

namespace MovieTicketBooking.Authorization
{
    public class BookingOwnerHandler
        : AuthorizationHandler<BookingOwnerRequirement, Booking>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BookingOwnerRequirement requirement,
            Booking booking)
        {
            // Check whether the logged-in user is an Admin.
            if (context.User.IsInRole(UserRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Get the logged-in user's ID from the JWT.
            var userId = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Check whether the booking belongs to this user.
            if (!string.IsNullOrEmpty(userId) &&
                booking.ApplicationUserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
