using RentIt.Modules.Bookings.Application.Services;
using RentIt.Modules.Bookings.Infrastructure.Database;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Bookings.Infrastructure.Services;

internal class BookingsInboxService(BookingsDbContext dbContext) 
    : InboxService<BookingsDbContext>(dbContext), IBookingsInboxService
{
}
