#include "ApiLock.h"

namespace Linphone
{
    namespace BackEnd
    {
        // A mutex used to protect objects accessible from the API surface exposed by this DLL
        std::recursive_mutex g_apiLock;
    }
}