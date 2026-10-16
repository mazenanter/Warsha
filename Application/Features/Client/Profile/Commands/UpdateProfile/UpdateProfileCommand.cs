<<<<<<< HEAD
using Domain.Common;
=======
﻿using Domain.Common;
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8
using MediatR;

namespace Application.Features.Client.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Result>
    {
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8
