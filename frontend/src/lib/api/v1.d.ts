/**
 * This file was auto-generated by openapi-typescript.
 * Do not make direct changes to the file.
 */

export interface paths {
    "/api/Application/running-env": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": string;
                        "application/json": string;
                        "text/json": string;
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/internal-inventory": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: {
                    symbol?: string;
                };
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["InternalInventoryItem"][];
                        "application/json": components["schemas"]["InternalInventoryItem"][];
                        "text/json": components["schemas"]["InternalInventoryItem"][];
                    };
                };
            };
        };
        put: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["InternalInventoryItem"];
                    "application/json": components["schemas"]["InternalInventoryItem"];
                    "text/json": components["schemas"]["InternalInventoryItem"];
                    "application/*+json": components["schemas"]["InternalInventoryItem"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        post: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["AddInternalInventoryItemRequest"];
                    "application/json": components["schemas"]["AddInternalInventoryItemRequest"];
                    "text/json": components["schemas"]["AddInternalInventoryItemRequest"];
                    "application/*+json": components["schemas"]["AddInternalInventoryItemRequest"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/internal-inventory/items/history": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: {
                    symbol?: string;
                };
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["InternalInventoryItem"][];
                        "application/json": components["schemas"]["InternalInventoryItem"][];
                        "text/json": components["schemas"]["InternalInventoryItem"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/internal-inventory/items": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: {
                    symbol?: string;
                    creatingType?: components["schemas"]["CreatingType"];
                    status?: components["schemas"]["State"];
                };
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["InternalInventoryItem"][];
                        "application/json": components["schemas"]["InternalInventoryItem"][];
                        "text/json": components["schemas"]["InternalInventoryItem"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/internal-inventory/deactivate": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["InternalInventoryItem"];
                    "application/json": components["schemas"]["InternalInventoryItem"];
                    "text/json": components["schemas"]["InternalInventoryItem"];
                    "application/*+json": components["schemas"]["InternalInventoryItem"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/internal-inventory/activate": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["InternalInventoryItem"];
                    "application/json": components["schemas"]["InternalInventoryItem"];
                    "text/json": components["schemas"]["InternalInventoryItem"];
                    "application/*+json": components["schemas"]["InternalInventoryItem"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/internal-inventory/delete": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["InternalInventoryItem"];
                    "application/json": components["schemas"]["InternalInventoryItem"];
                    "text/json": components["schemas"]["InternalInventoryItem"];
                    "application/*+json": components["schemas"]["InternalInventoryItem"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/Inventory/inventory": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        post: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["InventoryRequest"];
                    "application/json": components["schemas"]["InventoryRequest"];
                    "text/json": components["schemas"]["InventoryRequest"];
                    "application/*+json": components["schemas"]["InventoryRequest"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": {
                            [key: string]: components["schemas"]["InventoryItem"][];
                        };
                        "application/json": {
                            [key: string]: components["schemas"]["InventoryItem"][];
                        };
                        "text/json": {
                            [key: string]: components["schemas"]["InventoryItem"][];
                        };
                    };
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/options/sources": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["StringSelectValue"][];
                        "application/json": components["schemas"]["StringSelectValue"][];
                        "text/json": components["schemas"]["StringSelectValue"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/options/creating-types": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["CreatingType"][];
                        "application/json": components["schemas"]["CreatingType"][];
                        "text/json": components["schemas"]["CreatingType"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/options/internal-inventory/statuses": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["State"][];
                        "application/json": components["schemas"]["State"][];
                        "text/json": components["schemas"]["State"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/settings/provider": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["ProviderSettingExtended"][];
                        "application/json": components["schemas"]["ProviderSettingExtended"][];
                        "text/json": components["schemas"]["ProviderSettingExtended"][];
                    };
                };
            };
        };
        put: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["ProviderSettingRequest"];
                    "application/json": components["schemas"]["ProviderSettingRequest"];
                    "text/json": components["schemas"]["ProviderSettingRequest"];
                    "application/*+json": components["schemas"]["ProviderSettingRequest"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        post: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["ProviderSettingRequest"];
                    "application/json": components["schemas"]["ProviderSettingRequest"];
                    "text/json": components["schemas"]["ProviderSettingRequest"];
                    "application/*+json": components["schemas"]["ProviderSettingRequest"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/settings/provider/{providerId}/activate": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        post: {
            parameters: {
                query?: never;
                header?: never;
                path: {
                    providerId: string;
                };
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/settings/provider/{providerId}": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        post?: never;
        delete: {
            parameters: {
                query?: never;
                header?: never;
                path: {
                    providerId: string;
                };
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/quote": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        post: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["QuoteRequest"];
                    "application/json": components["schemas"]["QuoteRequest"];
                    "text/json": components["schemas"]["QuoteRequest"];
                    "application/*+json": components["schemas"]["QuoteRequest"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/QuoteDetails": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: {
                    quoteId?: string;
                };
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/report/locates": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        post: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json-patch+json": components["schemas"]["LocatesReportDataRequest"];
                    "application/json": components["schemas"]["LocatesReportDataRequest"];
                    "text/json": components["schemas"]["LocatesReportDataRequest"];
                    "application/*+json": components["schemas"]["LocatesReportDataRequest"];
                };
            };
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["LocatesReportData"][];
                        "application/json": components["schemas"]["LocatesReportData"][];
                        "text/json": components["schemas"]["LocatesReportData"][];
                    };
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/help/sse/locate-request": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["LocateRequestModel"];
                        "application/json": components["schemas"]["LocateRequestModel"];
                        "text/json": components["schemas"]["LocateRequestModel"];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/help/sse/locate-request-history": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["LocateRequestModel"][];
                        "application/json": components["schemas"]["LocateRequestModel"][];
                        "text/json": components["schemas"]["LocateRequestModel"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/help/sse/locate": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["LocateModel"];
                        "application/json": components["schemas"]["LocateModel"];
                        "text/json": components["schemas"]["LocateModel"];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/help/sse/locate-history": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["LocateModel"][];
                        "application/json": components["schemas"]["LocateModel"][];
                        "text/json": components["schemas"]["LocateModel"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/help/sse/notification": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["GroupedNotification"][];
                        "application/json": components["schemas"]["GroupedNotification"][];
                        "text/json": components["schemas"]["GroupedNotification"][];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/api/help/sse/internal-inventory": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "text/plain": components["schemas"]["InternalInventoryItem"];
                        "application/json": components["schemas"]["InternalInventoryItem"];
                        "text/json": components["schemas"]["InternalInventoryItem"];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
}
export type webhooks = Record<string, never>;
export interface components {
    schemas: {
        AddInternalInventoryItemRequest: {
            symbol: string;
            /** Format: int32 */
            quantity: number;
            /** Format: double */
            price: number;
            source: string;
            creatingType: components["schemas"]["CreatingType"];
        };
        AutoDisabledInfo: {
            symbols: string[] | null;
        };
        /** @enum {string} */
        CreatingType: "Unknown" | "SingleEntry" | "MultiEntry" | "Buy" | "CoverNegative" | "Overbuy" | "UnwantedPartial";
        GroupedNotification: {
            type: components["schemas"]["NotificationType"];
            kind: string;
            groupParameters: string;
            lastMessage: string;
            /** Format: date-time */
            firstTime: string;
            /** Format: date-time */
            lastTime: string;
            /** Format: int32 */
            count: number;
        };
        InternalInventoryItem: {
            id: string;
            /** Format: int32 */
            version: number;
            symbol: string;
            /** Format: int32 */
            quantity: number;
            /** Format: int32 */
            soldQuantity: number;
            /** Format: double */
            price: number;
            source: string;
            creatingType: components["schemas"]["CreatingType"];
            tag?: string | null;
            coveredInvItemId?: string | null;
            status: components["schemas"]["State"];
            /** Format: date-time */
            createdAt: string;
        };
        InventoryItem: {
            id: string;
            /** Format: int32 */
            version: number;
            accountId: string;
            symbol: string;
            /** Format: int32 */
            locatedQuantity: number;
            /** Format: int32 */
            availableQuantity: number;
        };
        InventoryRequest: {
            accountId: string;
        };
        LocateModel: {
            quoteId: string;
            accountId: string;
            /** Format: date-time */
            time: string;
            symbol: string;
            /** Format: int32 */
            reqQty: number;
            /** Format: int32 */
            qtyFill: number;
            /** Format: double */
            discountedPrice: number;
            /** Format: double */
            price: number;
            status: components["schemas"]["QuoteResponseStatusEnum"];
            errorMessage?: string | null;
            source: string;
            sourceDetails: components["schemas"]["QuoteSourceInfo"][];
        };
        LocateRequestModel: {
            id: string;
            accountId: string;
            /** Format: date-time */
            time: string;
            symbol: string;
            /** Format: int32 */
            qtyReq: number;
            /** Format: int32 */
            qtyOffer: number;
            /** Format: double */
            price: number;
            /** Format: double */
            discountedPrice: number;
            source: string;
            sourceDetails: components["schemas"]["QuoteSourceInfo"][];
        };
        LocatesReportData: {
            id: string;
            accountId: string;
            symbol: string;
            status: components["schemas"]["QuoteResponseStatusEnum"];
            /** Format: date-time */
            time: string;
            /** Format: int32 */
            reqQty: number;
            /** Format: int32 */
            fillQty?: number | null;
            /** Format: double */
            price?: number | null;
            /** Format: double */
            discountedPrice?: number | null;
            /** Format: double */
            fee?: number | null;
            /** Format: double */
            discountedFee?: number | null;
            /** Format: double */
            profit?: number | null;
            source: string;
            sources: components["schemas"]["QuoteSourceInfo"][];
            errorMessage?: string | null;
            /** Format: int32 */
            totalCount: number;
        };
        LocatesReportDataRequest: {
            /** Format: int32 */
            skip?: number | null;
            /** Format: int32 */
            take?: number | null;
            orderBy: string;
            /** Format: date-time */
            from?: string | null;
            /** Format: date-time */
            to?: string | null;
            status?: components["schemas"]["QuoteResponseStatusEnum"];
            symbol?: string | null;
            accountId?: string | null;
            providerId?: string | null;
        };
        /** @enum {string} */
        NotificationType: "Warning" | "Error" | "Critical";
        ProviderSettingExtended: {
            autoDisabled?: components["schemas"]["AutoDisabledInfo"];
            providerId: string;
            name: string;
            /** Format: double */
            discount: number;
            active: boolean;
            quoteRequestTopic?: string | null;
            quoteResponseTopic?: string | null;
            buyRequestTopic?: string | null;
            buyResponseTopic?: string | null;
        };
        ProviderSettingRequest: {
            providerId: string;
            name: string;
            /** Format: double */
            discount: number;
            active: boolean;
        };
        QuoteRequest: {
            id: string;
            requestType: components["schemas"]["RequestTypeEnum"];
            symbol: string;
            /** Format: int32 */
            quantity: number;
            accountId: string;
            allowPartial: boolean;
            autoApprove: boolean;
            /** Format: double */
            maxPriceForAutoApprove: number;
            /** Format: date-time */
            time: string;
        };
        /** @enum {string} */
        QuoteResponseStatusEnum: "Cancelled" | "Expired" | "Failed" | "RejectedBadRequest" | "RejectedDuplicate" | "WaitingAcceptance" | "Partial" | "Filled" | "NoInventory" | "AutoAccepted" | "AutoRejected" | "RequestAccepted" | "RejectedProhibited";
        QuoteSourceInfo: {
            provider: string;
            source: string;
            /** Format: double */
            price: number;
            /** Format: int32 */
            qty: number;
            /** Format: double */
            discountedPrice: number;
        };
        /** @enum {string} */
        RequestTypeEnum: "QuoteRequest" | "QuoteAccept" | "QuoteCancel";
        /** @enum {string} */
        State: "Active" | "Inactive" | "Deleted";
        StringSelectValue: {
            value: string | null;
            label: string;
        };
    };
    responses: never;
    parameters: never;
    requestBodies: never;
    headers: never;
    pathItems: never;
}
export type AddInternalInventoryItemRequest = components['schemas']['AddInternalInventoryItemRequest'];
export type AutoDisabledInfo = components['schemas']['AutoDisabledInfo'];
export type CreatingType = components['schemas']['CreatingType'];
export type GroupedNotification = components['schemas']['GroupedNotification'];
export type InternalInventoryItem = components['schemas']['InternalInventoryItem'];
export type InventoryItem = components['schemas']['InventoryItem'];
export type InventoryRequest = components['schemas']['InventoryRequest'];
export type LocateModel = components['schemas']['LocateModel'];
export type LocateRequestModel = components['schemas']['LocateRequestModel'];
export type LocatesReportData = components['schemas']['LocatesReportData'];
export type LocatesReportDataRequest = components['schemas']['LocatesReportDataRequest'];
export type NotificationType = components['schemas']['NotificationType'];
export type ProviderSettingExtended = components['schemas']['ProviderSettingExtended'];
export type ProviderSettingRequest = components['schemas']['ProviderSettingRequest'];
export type QuoteRequest = components['schemas']['QuoteRequest'];
export type QuoteResponseStatusEnum = components['schemas']['QuoteResponseStatusEnum'];
export type QuoteSourceInfo = components['schemas']['QuoteSourceInfo'];
export type RequestTypeEnum = components['schemas']['RequestTypeEnum'];
export type State = components['schemas']['State'];
export type StringSelectValue = components['schemas']['StringSelectValue'];
export type $defs = Record<string, never>;
export type operations = Record<string, never>;
