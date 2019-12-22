import reducer from './credentials';

describe('credentials reducer', () => {
    describe('SET_AWS_COGNITO_CREDENTIALS', () => {
        const fakeCredential = {
            fake:'',
            credential:'',
            object:''
        }
        const payload = {
            credentials: fakeCredential
        };

        it('sets credential properly', () => {
            const { credentials, authenticated } = reducer(undefined, {
                type: 'SET_AWS_COGNITO_CREDENTIALS',
                payload
            })

            expect(credentials).toMatchObject(fakeCredential);
            expect(authenticated).toEqual(true);
        });
    });
});