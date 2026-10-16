<<<<<<< HEAD
using Domain.Common;
=======
﻿using Domain.Common;
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8
using MediatR;

namespace Application.Features.Client.Reviews.Commands.AddReview
{
    public class AddReviewCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8
