using CeciRMQ.Domain.DTO.Commons;

namespace CeciRMQ.Domain.DTO.Address
{
    public class AddressFilterDTO : QueryFilter
    {
        public string District { get; set; }

        public string Locality { get; set; }

        public string Uf { get; set; }
    }
}
