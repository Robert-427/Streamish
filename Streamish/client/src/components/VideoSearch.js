export const VideoSearch = ({ setterFunction }) => {
    return (
        <div>
            <input
                onChange={(changeEvent) => {
                    setterFunction(changeEvent.target.value)
                }
                }
            type="text" placeholder="Search" />
        </div>
    )
}