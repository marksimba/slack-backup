import reducer from './app';

describe('app reducer', () => {
    describe('SET_URL_PARAMS', () => {

        const payload = {
            params: {
                testParamKey: "testParamValue"
            }
        };

        it('sets params properly', () => {
            const { params } = reducer(undefined, {
                type: 'SET_URL_PARAMS',
                payload
            })

            expect(params.testParamKey).toEqual("testParamValue");
        });
    });

    describe('SET_CHANNELS', () => {

        const payload = {
            channels: [
                {value: JSON.stringify({
                    id: "fakeID",
                    name: "fakeName"
                })},
                {value: JSON.stringify({
                    id: "fakeID2",
                    name: "fakeName2"
                })}
            ]
        };

        it('converts channels properly', () => {
            const { channels } = reducer(undefined, {
                type: 'SET_CHANNELS',
                payload
            })

            expect(channels.fakeID.name).toEqual("fakeName");
            expect(channels.fakeID2.name).toEqual("fakeName2");
        });
    });

    describe('SET_USERS', () => {

        const payload = {
            users: [
                {value: JSON.stringify({
                    id: "fakeID",
                    name: "fakeName"
                })},
                {value: JSON.stringify({
                    id: "fakeID2",
                    name: "fakeName2"
                })}
            ]
        };

        it('converts users properly', () => {
            const { users } = reducer(undefined, {
                type: 'SET_USERS',
                payload
            })

            expect(users.fakeID.name).toEqual("fakeName");
            expect(users.fakeID2.name).toEqual("fakeName2");
        });
    });

    describe('SET_MESSAGES', () => {

        const payload = {
            messages: [
                {value: JSON.stringify({
                    id: "fakeID",
                    ts: "1565931581.011600"
                })},
                {value: JSON.stringify({
                    id: "fakeID2",
                    ts: "1568317352.004300"
                })},
                {value: JSON.stringify({
                    id: "fakeID2",
                    ts: "1573665958.000900"
                })}
            ]
        };

        it('sets params properly', () => {
            const { messages } = reducer(undefined, {
                type: 'SET_MESSAGES',
                payload
            })

            expect(messages[0].ts).toEqual("1573665958.000900");
            expect(messages[1].ts).toEqual("1568317352.004300");
            expect(messages[2].ts).toEqual("1565931581.011600");
        });
    });
});